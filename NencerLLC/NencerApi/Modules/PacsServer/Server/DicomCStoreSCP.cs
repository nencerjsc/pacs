using FellowOakDicom;
using FellowOakDicom.Network;
using Serilog;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using FellowOakDicom.Network.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using Hl7.Fhir.Rest;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using NencerApi.Modules.PacsServer.Model;
using NencerCore;
using NencerApi.Modules.PacsServer.Service;
using NencerApi.Modules.PacsServer.Config;
using NencerApi.Modules.PacsServer.Helpers;

namespace NencerApi.Modules.PacsServer.Server
{
    public class DicomCStoreSCP : FellowOakDicom.Network.DicomService, IDicomServiceProvider, IDicomCStoreProvider, IDicomCEchoProvider, IDicomCFindProvider, IDicomCMoveProvider, IDicomCGetProvider
    {
        private readonly string _storageFolder = "dicom_storage";
        private readonly DicomWorkListService _workListService;
        public DicomCStoreSCP(INetworkStream stream, Encoding fallbackEncoding, DicomServiceDependencies dependencies)
        : base(stream, fallbackEncoding, new Serilog.Extensions.Logging.SerilogLoggerProvider().CreateLogger("DicomCStoreSCP"), dependencies)
        {
            Log.Information("🚀 Khởi tạo kết nối mới.");
            _storageFolder = AppConfig.DicomServer.StoragePath;
            Directory.CreateDirectory(_storageFolder);
            //_workListService = userState as DicomWorkListService;
        }

        public DicomCStoreSCP(
            INetworkStream stream,
            Encoding fallbackEncoding,
            Microsoft.Extensions.Logging.ILogger logger,
            DicomServiceDependencies dependencies)
            : base(stream, fallbackEncoding, logger, dependencies)
        {
            Log.Information("🚀 DicomCStoreSCP đã được khởi tạo");
            _storageFolder = AppConfig.DicomServer.StoragePath;
        }

        private static readonly DicomTransferSyntax[] _acceptedTransferSyntaxes = new DicomTransferSyntax[]
        {
               DicomTransferSyntax.ExplicitVRLittleEndian,
               DicomTransferSyntax.ExplicitVRBigEndian,
               DicomTransferSyntax.ImplicitVRLittleEndian
        };

        private static readonly DicomTransferSyntax[] _acceptedImageTransferSyntaxes = new DicomTransferSyntax[]
        {
               // Lossless
               DicomTransferSyntax.JPEGLSLossless,
               DicomTransferSyntax.JPEG2000Lossless,
               DicomTransferSyntax.JPEGProcess14SV1,
               DicomTransferSyntax.JPEGProcess14,
               DicomTransferSyntax.RLELossless,
               // Lossy
               DicomTransferSyntax.JPEGLSNearLossless,
               DicomTransferSyntax.JPEG2000Lossy,
               DicomTransferSyntax.JPEGProcess1,
               DicomTransferSyntax.JPEGProcess2_4,
               // Uncompressed
               DicomTransferSyntax.ExplicitVRLittleEndian,
               DicomTransferSyntax.ExplicitVRBigEndian,
               DicomTransferSyntax.ImplicitVRLittleEndian
        };

        public async Task OnReceiveAssociationRequestAsync(DicomAssociation association)
        {
            // Kiểm tra AETitle hoặc các tiêu chí xác thực
            var callingAE = association.CallingAE;
            var allowedAEs = AppConfig.DicomServer.AllowedAEs;

            if (!allowedAEs.Contains(callingAE))
            {
                Log.Warning("⛔ Từ chối kết nối từ AETitle không hợp lệ: {AE}", callingAE);

                await SendAssociationRejectAsync(
                    DicomRejectResult.Permanent,
                    DicomRejectSource.ServiceUser,
                    DicomRejectReason.CallingAENotRecognized);
                return;
            }

            foreach (var pc in association.PresentationContexts)
            {
                pc.AcceptTransferSyntaxes(DicomTransferSyntax.ImplicitVRLittleEndian);
            }

            Log.Information("📡 Nhận yêu cầu kết nối từ AE: {CallingAE}", association.CallingAE);
            await SendAssociationAcceptAsync(association);
        }



        public Task OnReceiveAssociationReleaseRequestAsync()
        {
            Log.Information("🔄 Nhận yêu cầu kết thúc kết nối (AssociationRelease) từ client.");
            return SendAssociationReleaseResponseAsync();
        }

        public void OnReceiveAbort(DicomAbortSource source, DicomAbortReason reason)
        {
            Log.Warning("🔴 Client hủy kết nối: {Source} - {Reason}", source, reason);
        }

        public void OnConnectionClosed(Exception exception)
        {
            var aeTitle = Association?.CallingAE ?? "Không xác định";
            if (exception != null)
            {
                Log.Error(exception, $"⚠️ Kết nối đến đến AE {aeTitle} bị đóng do lỗi.");
            }
            else
            {
                Log.Information($"🔌 Đóng kết nối đến AE {aeTitle} bình thường.");
            }

        }

        public Task<DicomCEchoResponse> OnCEchoRequestAsync(DicomCEchoRequest request)
        {
            Log.Information($"📶 Nhận yêu cầu C-ECHO từ AE {Association.CallingAE}.");
            return Task.FromResult(new DicomCEchoResponse(request, DicomStatus.Success));
        }

        public async Task<DicomCStoreResponse> OnCStoreRequestAsync(DicomCStoreRequest request)
        {
            Log.Information("📤 Nhận yêu cầu gửi tệp DICOM từ AE: {AETitle}", Association.CallingAE);
            try
            {
                var dicomFile = request.File;
                var dataset = dicomFile.Dataset;

                // Đưa file vào hàng đợi để xử lý sau
                DicomCStoreProcessorService.Instance.Enqueue(dicomFile);

                return new DicomCStoreResponse(request, DicomStatus.Success);

            }
            catch (Exception ex)
            {
                Log.Error(ex, "❌ Lỗi khi lưu ảnh DICOM.");
                return new DicomCStoreResponse(request, DicomStatus.ProcessingFailure);
            }
        }

        public Task OnCStoreRequestExceptionAsync(string tempFileName, Exception ex)
        {
            Log.Error(ex, "❌ Lỗi trong quá trình xử lý ảnh tạm thời: {File}", tempFileName);
            return Task.CompletedTask;
        }
        public async IAsyncEnumerable<DicomCFindResponse> OnCFindRequestAsync(DicomCFindRequest request)
        {
            Log.Information("📋 Nhận C-FIND từ AE: {AE}", Association?.CallingAE ?? string.Empty);

            var sopClass = request.SOPClassUID;

            if (sopClass == DicomUID.ModalityWorklistInformationModelFind)
            {
                // Nếu là Worklist, gọi hàm xử lý riêng và yield từng kết quả
                await foreach (var response in HandleWorklistAsync(request))
                {
                    yield return response;
                }
                yield break;
            }

            var storagePath = AppConfig.DicomServer.StoragePath;
            if (!Directory.Exists(storagePath))
            {
                Log.Error("🚫 DICOM directory not found");
                yield return new DicomCFindResponse(request, DicomStatus.StorageCannotUnderstand);
                yield break;
            }

            var dicomFiles = Directory.GetFiles(storagePath, "*.dcm");
            if (dicomFiles.Length == 0)
            {
                Log.Error("🚫 No DICOM files found");
                yield return new DicomCFindResponse(request, DicomStatus.NoSuchObjectInstance);
                yield break;
            }
            foreach (var file in dicomFiles)
            {
                var dicomFile = await DicomFile.OpenAsync(file);
                var dataset = dicomFile.Dataset;
                var response = new DicomCFindResponse(request, DicomStatus.Pending) { Dataset = dataset };
                yield return response;
            }
            yield return new DicomCFindResponse(request, DicomStatus.Success);
        }

        public async IAsyncEnumerable<DicomCMoveResponse> OnCMoveRequestAsync(DicomCMoveRequest request)
        {
            Log.Information("📥 Nhận C-MOVE từ client: {AE}", Association?.CallingAE ?? string.Empty);

            DicomDataset dataset = request.Dataset;
            string studyInstanceUID = dataset.GetSingleValue<string>(DicomTag.StudyInstanceUID);
            //string seriesInstanceUID = dataset.GetSingleValue<string>(DicomTag.SeriesInstanceUID);
            //string sopInstanceUID = dataset.GetSingleValue<string>(DicomTag.SOPInstanceUID);

            // Lấy đường dẫn thư mục DICOM
            var dicomDirectory = AppConfig.DicomServer.StoragePath;

            if (!Directory.Exists(dicomDirectory))
            {
                Log.Error("🚫 DICOM directory not found");
                yield return new DicomCMoveResponse(request, DicomStatus.Success);
                yield break;
            }

            var dicomFiles = Directory.GetFiles(dicomDirectory, "*.dcm");

            if (dicomFiles.Length == 0)
            {
                Log.Error("🚫 No DICOM files found");
                yield return new DicomCMoveResponse(request, DicomStatus.Success);
                yield break;
            }

            foreach (var file in dicomFiles)
            {

                var dicomFile = await DicomFile.OpenAsync(file);
                bool isMatch = true;

                if (!string.IsNullOrEmpty(studyInstanceUID))
                {
                    isMatch = dicomFile.Dataset.GetString(DicomTag.StudyInstanceUID) == studyInstanceUID;
                }
                if (isMatch)
                {
                    var cStoreRequest = new DicomCStoreRequest(file);
                    await SendRequestAsync(cStoreRequest);
                    yield return new DicomCMoveResponse(request, DicomStatus.Pending);
                }
            }
            yield return new DicomCMoveResponse(request, DicomStatus.Success);

        }
        public async IAsyncEnumerable<DicomCGetResponse> OnCGetRequestAsync(DicomCGetRequest request)
        {
            Log.Information("📥 Nhận C-GET từ client: {AE}", Association?.CallingAE ?? string.Empty);

            DicomDataset dataset = request.Dataset;
            string studyInstanceUID = dataset.GetSingleValue<string>(DicomTag.StudyInstanceUID);

            // Lấy đường dẫn thư mục DICOM
            var dicomDirectory = AppConfig.DicomServer.StoragePath;

            if (!Directory.Exists(dicomDirectory))
            {
                Log.Error("🚫 DICOM directory not found");
                yield return new DicomCGetResponse(request, DicomStatus.Success);
                yield break;
            }

            var dicomFiles = Directory.GetFiles(dicomDirectory, "*.dcm");

            if (dicomFiles.Length == 0)
            {
                Log.Error("🚫 No DICOM files found");
                yield return new DicomCGetResponse(request, DicomStatus.Success);
                yield break;
            }
            foreach (var file in dicomFiles)
            {

                var dicomFile = await DicomFile.OpenAsync(file);
                bool isMatch = true;

                if (!string.IsNullOrEmpty(studyInstanceUID))
                {
                    isMatch = dicomFile.Dataset.GetString(DicomTag.StudyInstanceUID) == studyInstanceUID;
                }
                if (isMatch)
                {
                    var cStoreRequest = new DicomCStoreRequest(file);
                    await SendRequestAsync(cStoreRequest);
                    yield return new DicomCGetResponse(request, DicomStatus.Pending);
                }
            }
            yield return new DicomCGetResponse(request, DicomStatus.Success);
        }
        public async IAsyncEnumerable<DicomCFindResponse> HandleWorklistAsync(DicomCFindRequest request)
        {
            Log.Information("📋 Nhận C-FIND Worklist từ AE: {AE}", Association?.CallingAE ?? string.Empty);

            //foreach (var item in request.Dataset)
            //{
            //    if (item is DicomElement element)
            //    {
            //        string value = element.Get<string>();
            //        Log.Information($"  {item.Tag} [{item.Tag.DictionaryEntry.Name}]: {value}");
            //    }
            //    else
            //    {
            //        Log.Information($"  {item.Tag} [{item.Tag.DictionaryEntry.Name}]: <non-element or empty>");
            //    }
            //}


            if (string.IsNullOrEmpty(Association?.CallingAE))
            {
                Log.Warning("⚠️ Không có CallingAE trong yêu cầu C-FIND, từ chối truy vấn.");
                yield return new DicomCFindResponse(request, DicomStatus.NoSuchObjectInstance);
                yield break;
            }

            List<DicomWorkListModel> worklistItems = new List<DicomWorkListModel>();
            try
            {
                string? AEC = Association?.CallingAE;

                using var _context = new AppDbContext();

                var query = _context.DicomWorkLists.AsQueryable();
                query = query.Where(w => w.ScheduledAET == AEC);

                if (request.Dataset.TryGetSingleValue(DicomTag.RequestedProcedureID, out string requestedProcedureID) &&
                    !string.IsNullOrWhiteSpace(requestedProcedureID))
                {
                    query = query.Where(w => w.RequestedProcedureID == requestedProcedureID);
                }

                if (request.Dataset.TryGetSingleValue(DicomTag.RequestedProcedureDescription, out string requestedProcedureDescription) &&
                    !string.IsNullOrWhiteSpace(requestedProcedureDescription))
                {
                    query = query.Where(w => w.RequestedProcedureDescription == requestedProcedureDescription);
                }

                if (request.Dataset.TryGetSingleValue(DicomTag.StudyInstanceUID, out string studyInstanceUID) &&
                    !string.IsNullOrWhiteSpace(studyInstanceUID))
                {
                    query = query.Where(w => w.StudyInstanceUID == studyInstanceUID);
                }

                if (request.Dataset.TryGetSingleValue(DicomTag.Modality, out string modality) &&
                    !string.IsNullOrWhiteSpace(modality))
                {
                    query = query.Where(w => w.Modality == modality);
                }

                if (request.Dataset.TryGetSingleValue(DicomTag.AccessionNumber, out string accessionNumber) &&
                    !string.IsNullOrWhiteSpace(accessionNumber))
                {
                    query = query.Where(w => w.AccessionNumber == accessionNumber);
                }

                if (request.Dataset.TryGetSingleValue(DicomTag.PatientID, out string patientId) &&
                    !string.IsNullOrWhiteSpace(patientId))
                {
                    query = query.Where(w => w.PatientID == patientId);
                }

                if (request.Dataset.TryGetSingleValue(DicomTag.PatientName, out string patientName) &&
                    !string.IsNullOrWhiteSpace(patientName))
                {
                    query = query.Where(w => w.PatientName == patientName);
                }

                if (request.Dataset.TryGetSingleValue(DicomTag.ScheduledStationAETitle, out string scheduledAET) &&
                    !string.IsNullOrWhiteSpace(scheduledAET))
                {
                    query = query.Where(w => w.ScheduledAET == scheduledAET);
                }

                if (request.Dataset.TryGetSingleValue(DicomTag.StudyDate, out string studyDate) &&
                    !string.IsNullOrWhiteSpace(studyDate))
                {
                    query = query.Where(w => w.StudyDate == studyDate);
                }


                worklistItems = query.ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "❌ Lỗi khi truy vấn Worklist từ cơ sở dữ liệu.");
                yield break;
            }

            //DicomWorkListService _workListService = new DicomWorkListService();
            //var worklistItems = await _workListService.GetAllAsync();

            if (worklistItems == null || worklistItems.Count == 0)
            {
                yield return new DicomCFindResponse(request, DicomStatus.NoSuchObjectInstance);
                yield break;
            }

            foreach (var wl in worklistItems)
            {
                DicomDataset dicomDataset = CreateListDataset(wl);

                yield return new DicomCFindResponse(request, DicomStatus.Pending) { Dataset = dicomDataset };
            }

            yield return new DicomCFindResponse(request, DicomStatus.Success);
        }

        public DicomDataset CreateListDataset(DicomWorkListModel wl)
        {
            var dicomDataset = new DicomDataset();

            void TryAdd(DicomDataset ds, DicomTag tag, string? value, string? defaultValue = null)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(value))
                        ds.Add(tag, value);
                    else if (defaultValue != null)
                        ds.Add(tag, defaultValue);
                    // Nếu cả value và defaultValue đều rỗng → bỏ qua
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, $"⚠️ Không thể gán tag {tag} ({tag.DictionaryEntry.Name}) với giá trị: '{value}'");
                }
            }

            try
            {
                // Bệnh nhân
                TryAdd(dicomDataset, DicomTag.PatientName, wl.PatientName);
                TryAdd(dicomDataset, DicomTag.PatientID, wl.PatientID, "UNKNOWN");
                TryAdd(dicomDataset, DicomTag.PatientSex, wl.PatientSex);
                TryAdd(dicomDataset, DicomTag.PatientBirthDate, wl.PatientBirthDate);

                // Thủ tục
                TryAdd(dicomDataset, DicomTag.AccessionNumber, wl.AccessionNumber);
                TryAdd(dicomDataset, DicomTag.StudyInstanceUID, wl.StudyInstanceUID, DicomUID.Generate().UID);
                TryAdd(dicomDataset, DicomTag.RequestedProcedureDescription, wl.RequestedProcedureDescription);
                TryAdd(dicomDataset, DicomTag.Modality, wl.Modality, "OT");
                TryAdd(dicomDataset, DicomTag.RequestedProcedureID, wl.RequestedProcedureID, "PROC001");

                // Thời gian
                TryAdd(dicomDataset, DicomTag.ScheduledProcedureStepStartDate, wl.ScheduledProcedureStepStartDate, DateTime.Now.ToString("yyyyMMdd"));
                TryAdd(dicomDataset, DicomTag.ScheduledProcedureStepStartTime, wl.ScheduledProcedureStepStartTime, DateTime.Now.ToString("HHmmss"));

                // Bác sĩ
                TryAdd(dicomDataset, DicomTag.ReferringPhysicianName, wl.ReferringPhysician);
                TryAdd(dicomDataset, DicomTag.PerformingPhysicianName, wl.PerformingPhysician);

                // Máy
                TryAdd(dicomDataset, DicomTag.ScheduledStationAETitle, wl.ScheduledAET);
                TryAdd(dicomDataset, DicomTag.ScheduledStationName, wl.ScheduledStationName);

                // ScheduledProcedureStepSequence
                var sps = new DicomDataset();
                TryAdd(sps, DicomTag.ScheduledStationAETitle, wl.ScheduledAET);
                TryAdd(sps, DicomTag.ScheduledProcedureStepDescription, wl.StudyDescription);
                TryAdd(sps, DicomTag.ScheduledProcedureStepStartDate, wl.ScheduledProcedureStepStartDate, DateTime.Now.ToString("yyyyMMdd"));
                TryAdd(sps, DicomTag.ScheduledProcedureStepStartTime, wl.ScheduledProcedureStepStartTime, DateTime.Now.ToString("HHmmss"));
                TryAdd(sps, DicomTag.ScheduledPerformingPhysicianName, wl.PerformingPhysician);
                TryAdd(sps, DicomTag.ScheduledStationName, wl.ScheduledStationName);

                dicomDataset.Add(DicomTag.ScheduledProcedureStepSequence, sps);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "❌ Lỗi khi tạo DicomDataset.");
            }

            return dicomDataset;
        }
    }
}

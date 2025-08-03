using FellowOakDicom;
using Hl7.Fhir.Rest;
using NencerApi.Modules.PacsServer.Helpers;
using NencerApi.Modules.PacsServer.Model;
using Serilog;
using System.Threading.Channels;

namespace NencerApi.Modules.PacsServer.Service
{
    public class DicomCStoreProcessorService
    {
        private readonly Channel<DicomFile> _channel;
        private readonly StoragePathService _storagePathService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DicomCStoreProcessorService(StoragePathService storagePathService, IServiceScopeFactory serviceScopeFactory)
        {
            _storagePathService = storagePathService;
            _serviceScopeFactory = serviceScopeFactory;
            _channel = Channel.CreateUnbounded<DicomFile>();
            _ = Task.Run(() => StartConsumer());
        }

        public void Enqueue(DicomFile dicomFile)
        {
            _channel.Writer.TryWrite(dicomFile);
        }

        private async Task StartConsumer()
        {
            await foreach (var dicomFile in _channel.Reader.ReadAllAsync())
            {
                await ProcessDicomDatasetAsync(dicomFile);
            }
        }

        private async Task ProcessDicomDatasetAsync(DicomFile dicomFile)
        {
            try
            {
                var dataset = dicomFile.Dataset;
                string patientId = dataset.GetSingleValueOrDefault(DicomTag.PatientID, "UNKNOWN");
                string dicomDate = dataset.GetSingleValueOrDefault(DicomTag.StudyDate, DateTime.Now.ToString("yyyyMMdd"));

                if (dicomDate.Length < 8)
                    dicomDate = DateTime.Now.ToString("yyyyMMdd");

                var storagePathModel = await _storagePathService.GetActiveStorageAsync();
                if (storagePathModel == null)
                {
                    Log.Error("❌ Không có storage path đang được kích hoạt.");
                    return;
                }

                string fileName = dataset.GetSingleValue<string>(DicomTag.SOPInstanceUID) + ".dcm";
                string relativePath = Path.Combine(patientId, dicomDate, fileName);
                string fullPath = Path.Combine(storagePathModel.Path, relativePath);

                await SaveDicomFileAsync(dicomFile, fullPath);
                await SaveDicomMetadataAsync(dataset, storagePathModel.Id, relativePath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "❌ Lỗi khi xử lý DICOM dataset");
            }
        }

        private static async Task SaveDicomFileAsync(DicomFile dicomFile, string fullPath)
        {
            var dir = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir!);

            if (!File.Exists(fullPath))
            {
                await dicomFile.SaveAsync(fullPath);
                Log.Information("📁 Đã lưu file DICOM: {Path}", fullPath);
            }
            else
            {
                Log.Information("📁 File đã tồn tại: {Path}", fullPath);
            }
        }

        private async Task SaveDicomMetadataAsync(DicomDataset dataset, int storageId, string relativePath)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dicomDataService = scope.ServiceProvider.GetRequiredService<DicomDataService>();
            var converter = new DicomDataSetConvertHelper(dataset);

            string studyUID = dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, string.Empty);
            string seriesUID = dataset.GetSingleValueOrDefault(DicomTag.SeriesInstanceUID, string.Empty);
            string sopUID = dataset.GetSingleValueOrDefault(DicomTag.SOPInstanceUID, string.Empty);

            bool isNewStudy = false, isNewSeries = false, isNewInstance = false;

            // Study
            var study = await dicomDataService.GetStudyByInstanceUIDAsync(studyUID);
            if (study == null)
            {
                isNewStudy = true;
                study = converter.ConvertToStudyModel();
                study.NumberOfStudyRelatedSeries = 1;
                study.NumberOfStudyRelatedInstances = 1;
                await dicomDataService.CreateStudyAsync(study);
            }

            // Series
            if (!await dicomDataService.CheckExistsSerieInstanceUIDAsync(seriesUID))
            {
                isNewSeries = true;
                var series = converter.ConvertToSerieModel();
                await dicomDataService.CreateSerieAsync(series);
            }

            // Instance
            if (!await dicomDataService.CheckExistsSOPInstanceUIDAsync(sopUID))
            {
                isNewInstance = true;
                var instance = converter.ConvertToInstanceModel(relativePath);
                instance.StorageId = storageId;
                instance.FilePath = relativePath;
                await dicomDataService.CreateSOPInstanceAsync(instance);
            }

            // Update counters
            if (!isNewStudy && (isNewSeries || isNewInstance))
            {
                if (isNewSeries) study.NumberOfStudyRelatedSeries++;
                if (isNewInstance) study.NumberOfStudyRelatedInstances++;
                await dicomDataService.UpdateStudyAsync(study);
            }

            if (isNewInstance && !string.IsNullOrEmpty(seriesUID))
            {
                var series = await dicomDataService.GetSeriesByInstanceUIDAsync(seriesUID);
                if (series != null)
                {
                    series.NumberOfStudyRelatedInstances++;
                    await dicomDataService.UpdateSeriesAsync(series);
                }
            }

            if (!string.IsNullOrEmpty(sopUID))
            {
                var tags = converter.ConvertToDicomTagItemModelList();
                await dicomDataService.SaveDicomTagsAsync(sopUID, tags);
            }
        }

        
    }
}

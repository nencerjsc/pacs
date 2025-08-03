using DevExpress.XtraRichEdit.Model;
using FellowOakDicom.Network;
using FellowOakDicom;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NencerApi.Modules.PacsServer.Model;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using NencerApi.Helpers;
using NencerApi.Modules.PacsServer.Service;
using System.Globalization;
using System.Text.Json;
using NencerCore;
using Serilog;
using NencerApi.Modules.PacsServer.Model.Dto;

namespace NencerApi.Modules.PacsServer.Controller
{

    [ApiController]
    [Route("[controller]")]
    public class WorkListController : ControllerBase
    {
        private readonly FhirJsonSerializer _serializer = new();
        private readonly DicomWorkListService _worklistService;
        private readonly AppDbContext _context;

        public WorkListController(AppDbContext context, DicomWorkListService worklistService)
        {
            _worklistService = worklistService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _worklistService.GetAllAsync();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateWorkList([FromBody] WorklistRequestDTO wlDTO)
        {
            if (wlDTO == null)
            {
                // Trả về lỗi BadRequest nếu không có dữ liệu
                return BadRequest(new BadRequestResponse<WorklistRequestDTO>("Invalid worklist data"));
            }

            try
            {
                var wlModel = new DicomWorkListModel
                {
                    OrderRequestId = wlDTO.OrderId,
                    AccessionNumber = wlDTO.AccessionNumber,
                    Modality = wlDTO.Modality,
                    PatientName = wlDTO.PatientName,
                    PatientID = wlDTO.PatientId,
                    PatientSex = wlDTO.PatientSex,
                    PatientBirthDate = wlDTO.PatientBirthDate,
                    RequestedProcedureDescription = wlDTO.ScheduledProcedureStepDescription,
                    StudyDescription = wlDTO.StudyDescription,
                    ScheduledProcedureStepStartDate = wlDTO.ScheduledProcedureStepStartDate,
                    ScheduledProcedureStepStartTime = wlDTO.ScheduledProcedureStepStartTime,
                    ReferringPhysician = wlDTO.DoctorName,
                    PerformingPhysician = wlDTO.ScheduledPerformingPhysicianName,
                    ScheduledAET = wlDTO.ScheduledStationAETitle,
                    ScheduledStationName = wlDTO.ScheduledStationName,
                    ExamRoom = wlDTO.ExamRoom,
                    ExamDescription = wlDTO.ExamDescription,
                    HospitalName = wlDTO.HospitalName,
                    Node = wlDTO.Notes,
                    Status = WorklistStatus.Scheduled,
                    ScheduledProcedureStep = new List<DicomScheduledProcedureStepModel>
                    {
                        new DicomScheduledProcedureStepModel
                        {
                            Modality = wlDTO.Modality,
                            ScheduledStationAETitle = wlDTO.ScheduledStationAETitle,
                            ScheduledProcedureStepStartDate = wlDTO.ScheduledProcedureStepStartDate,
                            ScheduledProcedureStepStartTime = wlDTO.ScheduledProcedureStepStartTime,
                            ScheduledPerformingPhysicianName = wlDTO.ScheduledPerformingPhysicianName,
                            ScheduledProcedureStepDescription = wlDTO.ScheduledProcedureStepDescription,
                            ScheduledStationName = wlDTO.ScheduledStationName
                        }
                    }
                };

                // Kiểm tra xem đơn hàng đã tồn tại chưa
                if (await _worklistService.CheckIfWorkListExists(wlModel.OrderRequestId, wlModel.AccessionNumber))
                {
                    Log.Information("✅ Worklist đã tồn tại: " + wlModel.OrderRequestId);
                    var responsetExists = new BaseResponse<DicomWorkListModel>
                    {
                        Status = "200",
                        Message = $"OrderRequestId already exists: {wlModel.OrderRequestId}"
                    };
                    return Ok(responsetExists);
                }

                var createdWorkList = await _worklistService.CreateAsync(wlModel);
                var data = new DicomWorkListModel
                {
                    OrderRequestId = wlDTO.OrderId,
                    AccessionNumber = wlDTO.AccessionNumber,
                    Modality = wlDTO.Modality,
                    PatientName = wlDTO.PatientName,
                    PatientID = wlDTO.PatientId,
                    PatientSex = wlDTO.PatientSex,
                    PatientBirthDate = wlDTO.PatientBirthDate,
                    RequestedProcedureDescription = wlDTO.ScheduledProcedureStepDescription,
                    StudyDescription = wlDTO.StudyDescription,
                    ScheduledProcedureStepStartDate = wlDTO.ScheduledProcedureStepStartDate,
                    ScheduledProcedureStepStartTime = wlDTO.ScheduledProcedureStepStartTime,
                    ReferringPhysician = wlDTO.DoctorName,
                    PerformingPhysician = wlDTO.ScheduledPerformingPhysicianName,
                    ScheduledAET = wlDTO.ScheduledStationAETitle,
                    ScheduledStationName = wlDTO.ScheduledStationName,
                    ExamRoom = wlDTO.ExamRoom,
                    ExamDescription = wlDTO.ExamDescription,
                    HospitalName = wlDTO.HospitalName,
                    Node = wlDTO.Notes,
                };

                Log.Information("✅ Tạo Worklist thành công: " + wlModel.OrderRequestId);

                var response = new BaseResponse<DicomWorkListModel>
                {
                    Status = "200",
                    Message = "Success",
                    Data = data
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error("❌ Lỗi tạo Worklist: " + ex.Message);
                var errorResponse = new ExceptionErrorResponse<DicomWorkListModel>(ex);
                return StatusCode(500, errorResponse);
            }
        }

        // ✅ POST: api/Worklist
        [HttpPost("fhir")]
        public async Task<IActionResult> Create([FromBody] JsonElement json)
        {
            try
            {
                var parser = new FhirJsonParser();
                var bundle = parser.Parse<Bundle>(json.GetRawText());

                if (bundle.Type != Bundle.BundleType.Collection && bundle.Type != Bundle.BundleType.Transaction)
                    return BadRequest("Bundle type must be 'collection' or 'transaction'");

                foreach (var entry in bundle.Entry)
                {
                    if (entry.Resource is ServiceRequest request)
                    {
                        //var model = MapFhirToWorkList(request);
                        //var existing = await _worklistService.GetByAccessionNumberAsync(model.AccessionNumber);

                        //if (existing != null)
                        //{
                        //    model.Status = WorklistStatus.Scheduled;
                        //    await _worklistService.UpdateAsync(model.AccessionNumber, model);
                        //}
                        //else
                        //{
                        //    await _worklistService.CreateAsync(model);
                        //}
                    }
                }

                return Ok(new { message = "Worklist đã được xử lý" });
            }
            catch (Exception ex)
            {
                Log.Error("❌ Lỗi tạo Worklist: " + ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{accessionNumber}")]
        public async Task<IActionResult> Update(string accessionNumber, [FromBody] DicomWorkListModel model)
        {
            var result = await _worklistService.UpdateAsync(accessionNumber, model);
            if (!result) return NotFound();
            return Ok(new { message = "Đã cập nhật Worklist" });
        }

        [HttpDelete("{accessionNumber}")]
        public async Task<IActionResult> Delete(string accessionNumber)
        {
            var result = await _worklistService.DeleteAsync(accessionNumber);
            if (!result) return NotFound();
            return Ok(new { message = "Đã xóa Worklist" });
        }

        // 🎯 Hàm chuyển đổi từ FHIR ServiceRequest sang WorkListModel
        private DicomWorkListModel MapFhirToWorkList(ServiceRequest request)
        {
            var accessionNumber = request.Id ?? Guid.NewGuid().ToString();
            var patientId = request.Subject?.Reference?.Replace("Patient/", "") ?? "";

            var authoredDate = DateTimeOffset.TryParse(request.AuthoredOn, out var authored)
                ? authored.DateTime.ToString("yyyyMMdd", CultureInfo.InvariantCulture)
                : "";

            return new DicomWorkListModel
            {
                AccessionNumber = accessionNumber,
                OrderRequestId = request.Identifier?.FirstOrDefault()?.Value ?? "",
                PatientID = patientId,
                Modality = request.Category?.FirstOrDefault()?.Coding?.FirstOrDefault()?.Code ?? "CT",
                RequestedProcedureID = request.BasedOn?.FirstOrDefault()?.Reference ?? "",
                RequestedProcedureDescription = request.Code?.Text ?? request.Code?.Coding?.FirstOrDefault()?.Display ?? "",
                StudyInstanceUID = Guid.NewGuid().ToString(),
                StudyDate = authoredDate,
                PatientName = request.Subject?.Display ?? "",
                ExamRoom = request.LocationReference?.FirstOrDefault()?.Display ?? "Room 1",
                ExamDescription = request.Note?.FirstOrDefault()?.Text ?? "",
                HospitalName = request.Encounter?.Display ?? string.Empty,
                Status = WorklistStatus.New
            };
        }

    }
}

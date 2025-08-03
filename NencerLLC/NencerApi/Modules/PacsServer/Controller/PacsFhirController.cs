
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace NencerApi.Modules.PacsServer.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class PacsFhirController: ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> ReceiveBundle()
        {
            using var reader = new StreamReader(Request.Body);
            var json = await reader.ReadToEndAsync();

            try
            {
                var parser = new FhirJsonParser();
                var bundle = parser.Parse<Bundle>(json);

                Log.Information("📦 Nhận được Bundle gồm {Count} resource(s)", bundle.Entry.Count);

                foreach (var entry in bundle.Entry)
                {
                    if (entry.Resource is Resource resource)
                    {
                        Log.Information("➡️ Resource Type: {TypeName}", resource.TypeName);

                        switch (resource)
                        {
                            case Patient patient:
                                var patientId   = patient.Identifier.FirstOrDefault()?.Value;
                                var patientName = patient.Name?.FirstOrDefault();
                                var patientBirthDate = patient.BirthDate;
                                var PatientGender = patient.Gender;
                                break;

                            case ImagingStudy study:
                                var studyId = study.Id;
                                var studyAccessionNumber = study.Identifier?.FirstOrDefault()?.Value;
                                var studyInstanceUID = study.Identifier?[1].Value;
                                var studySeriesInstanceUID = study.Series?.FirstOrDefault()?.Uid;
                                var studySOPInstanceUID = study.Series?.FirstOrDefault()?.Instance.FirstOrDefault()?.Uid;
                                var studyStarted = study.Started;
                                break;

                            case ServiceRequest serviceRequest:
                                var serviceRequestId = serviceRequest.Id;
                                var serviceRequestAccessionNumber = serviceRequest.Identifier?.FirstOrDefault()?.Value;
                                var serviceRequestStatus = serviceRequest.Status;
                                var serviceRequestIntent = serviceRequest.Intent;
                                var serviceRequestCode = serviceRequest.Code?.Text ?? serviceRequest.Code?.Coding?.FirstOrDefault()?.Display;
                                var serviceRequestRequester = serviceRequest.Requester?.Reference;
                                var serviceRequestPatient = serviceRequest.Subject?.Reference;
                                var serviceRequestReason = string.Join("; ", serviceRequest.ReasonCode?.Select(r => r.Text) ?? new List<string>());
                                var serviceRequestAuthoredOn = serviceRequest.AuthoredOn;
                                break;

                            case Hl7.Fhir.Model.Endpoint endpoint:
                                var endpointId = endpoint.Id;
                                var address = endpoint.Address;
                                var connectionType = endpoint.ConnectionType;
                                var code = connectionType?.Code;
                                var display = connectionType?.Display;

                                Log.Information($"  📡 Endpoint ID: {endpointId}");
                                Log.Information($"      🔗 Address: {address}");
                                Log.Information($"      🔌 Connection type: {code} - {display}");

                                break;

                            default:                                
                                break;
                        }
                    }
                    else
                    {
                        // Log.Warning("❗ Resource không hợp lệ hoặc không parse được.");
                    }
                }

                // Tạo OperationOutcome theo chuẩn FHIR
                var outcome = new OperationOutcome
                {
                    Id = Guid.NewGuid().ToString(),
                    Meta = new Meta
                    {
                        LastUpdated = DateTimeOffset.Now
                    },
                    Issue = new List<OperationOutcome.IssueComponent>
                    {
                        new OperationOutcome.IssueComponent
                        {
                            Severity = OperationOutcome.IssueSeverity.Information,
                            Code = OperationOutcome.IssueType.Informational,
                            Diagnostics = "✅ Bundle đã được nhận và xử lý thành công.",
                            Details = new CodeableConcept
                            {
                                Text = "Xử lý thành công"
                            }
                        }
                    }
                };

                // Cấu hình serializer FHIR
                var serializer = new FhirJsonSerializer(new SerializerSettings
                {
                    Pretty = true,
                    AppendNewLine = true
                });

                // Serialize thành JSON chuẩn FHIR
                var jsonResponse = serializer.SerializeToString(outcome);

                // Trả về với content type application/fhir+json
                return Content(jsonResponse, "application/fhir+json");
            }
            catch (Exception ex)
            {
                var outcome = new OperationOutcome
                {
                    Id = Guid.NewGuid().ToString(),
                    Issue = new List<OperationOutcome.IssueComponent>
                    {
                        new OperationOutcome.IssueComponent
                        {
                            Severity = OperationOutcome.IssueSeverity.Error,
                            Code = OperationOutcome.IssueType.Exception,
                            Diagnostics = $"❌ Lỗi parse bundle: {ex.Message}"
                        }
                    }
                };

                return StatusCode(400, outcome); // ✅ lỗi cũng là FHIR JSON
            }
        }
    }
}

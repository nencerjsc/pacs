using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NencerApi.Modules.PacsServer.Model
{
    [Table("PacsDicomResults")]
    public class PacsDicomResultModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        // Thông tin bệnh nhân
        public string? PatientId { get; set; }
        public string? PatientName { get; set; }
        public string? PatientBirthDate { get; set; }
        public string? PatientGender { get; set; }

        // Thông tin hình ảnh
        public string? StudyId { get; set; }
        public string? StudyAccessionNumber { get; set; }
        public string? StudyInstanceUID { get; set; }
        public string? StudySeriesInstanceUID { get; set; }
        public string? StudySOPInstanceUID { get; set; }
        public DateTimeOffset? StudyStarted { get; set; }

        // Thông tin yêu cầu dịch vụ
        public string? ServiceRequestId { get; set; }
        public string? ServiceRequestAccessionNumber { get; set; }
        public string? ServiceRequestStatus { get; set; }
        public string? ServiceRequestIntent { get; set; }
        public string? ServiceRequestCode { get; set; }
        public string? ServiceRequestRequester { get; set; }
        public string? ServiceRequestPatient { get; set; }
        public string? ServiceRequestReason { get; set; }
        public DateTimeOffset? ServiceRequestAuthoredOn { get; set; }

        // Thông tin endpoint kết nối PACS
        public string? EndpointId { get; set; }
        public string? EndpointAddress { get; set; }
        public string? EndpointConnectionCode { get; set; }
        public string? EndpointConnectionDisplay { get; set; }

        // Thời gian tạo bản ghi
        public DateTime? CreatedAt { get; set; }
    }
}

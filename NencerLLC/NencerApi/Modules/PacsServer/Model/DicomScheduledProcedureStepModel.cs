using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NencerApi.Modules.PacsServer.Model
{
    [Table("DicomScheduledProcedureSteps")]
    public class DicomScheduledProcedureStepModel
    {
        // Khoá chính tự sinh (hoặc bạn có thể chọn UID nếu muốn)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Modality { get; set; } = string.Empty;
        public string ScheduledStationAETitle { get; set; } = string.Empty;
        public string ScheduledProcedureStepStartDate { get; set; } = string.Empty;
        public string ScheduledProcedureStepStartTime { get; set; } = string.Empty;
        public string ScheduledPerformingPhysicianName { get; set; } = string.Empty;
        public string ScheduledProcedureStepDescription { get; set; } = string.Empty;
        public string ScheduledStationName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

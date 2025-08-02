namespace NencerApi.Modules.PacsServer.Model
{
    public class DicomScheduledProcedureStepModel
    {
        // Khoá chính tự sinh (hoặc bạn có thể chọn UID nếu muốn)
        public int Id { get; set; }
        public string Modality { get; set; } = string.Empty;
        public string ScheduledStationAETitle { get; set; } = string.Empty;
        public string ScheduledProcedureStepStartDate { get; set; } = string.Empty;
        public string ScheduledProcedureStepStartTime { get; set; } = string.Empty;
        public string ScheduledPerformingPhysicianName { get; set; } = string.Empty;
        public string ScheduledProcedureStepDescription { get; set; } = string.Empty;
        public string ScheduledStationName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // 🔗 Khóa ngoại trỏ tới DicomWorkList (AccessionNumber)
        public int WorkListID { get; set; }

        // Navigation property
        public DicomWorkListModel WorkList { get; set; } = null!;
    }
}

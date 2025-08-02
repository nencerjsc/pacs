namespace NencerApi.Modules.PacsServer.Model
{
    public class DicomWorkListModel
    {
        public int Id { get; set; }
        public string OrderRequestId { get; set; } = string.Empty;
        public string AccessionNumber { get; set; } = string.Empty;
        public string Modality { get; set; } = string.Empty; // Loại hình chẩn đoán hình ảnh (CT, MRI, X-ray, ...)
        // Thông tin bệnh nhân
        public string PatientName { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Forename { get; set; } = string.Empty;
        public string PatientID { get; set; } = string.Empty;
        public string PatientSex { get; set; } = string.Empty;
        public string PatientBirthDate { get; set; } = string.Empty;

        // Thông tin về thủ tục yêu cầu        
        public string StudyInstanceUID { get; set; } = string.Empty;
        public string RequestedProcedureDescription { get; set; } = string.Empty;
        public string StudyDate { get; set; } = string.Empty;
        public string StudyDescription { get; set; } = string.Empty;
        public string RequestedProcedureID { get; set; } = string.Empty;

        // thời gian chỉ định
        public string ScheduledProcedureStepStartDate { get; set; } = string.Empty;
        public string ScheduledProcedureStepStartTime { get; set; } = string.Empty;

        // Bác sĩ
        public string ReferringPhysician { get; set; } = string.Empty;
        public string PerformingPhysician { get; set; } = string.Empty;

        // thông tin máy
        public string ScheduledAET { get; set; } = string.Empty;
        public string ScheduledStationName { get; set; } = string.Empty;  // Tên máy hoặc thiết bị

        // thông tin phòng
        public string ExamRoom { get; set; } = string.Empty;
        public string ExamDescription { get; set; } = string.Empty;
        public string HospitalName { get; set; } = string.Empty;
        public string Node { get; set; } = string.Empty;
        public WorklistStatus Status { get; set; } = WorklistStatus.New;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        // Scheduled Procedure Step
        public List<DicomScheduledProcedureStepModel> ScheduledProcedureStep { get; set; } = new List<DicomScheduledProcedureStepModel>();
        public int ServiceRequestId { get; set; }
        //public ServiceRequestModel ServiceRequest { get; set; } = null!;
    }

    public enum WorklistStatus
    {
        New = 0,
        Scheduled = 1,
        InProgress = 2,
        Completed = 3,
        Canceled = 4
    }

}

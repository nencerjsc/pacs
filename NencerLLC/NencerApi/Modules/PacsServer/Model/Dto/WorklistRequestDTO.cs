namespace NencerApi.Modules.PacsServer.Model.Dto
{
    public class WorklistRequestDTO
    {
        // --- Thông tin bệnh nhân ---
        public string PatientId { get; set; } = string.Empty;                         // Mã bệnh nhân duy nhất trong hệ thống HIS
        public string PatientName { get; set; } = string.Empty;                       // Họ tên đầy đủ bệnh nhân
        public string PatientSex { get; set; } = string.Empty;                        // Giới tính: M (Nam), F (Nữ), O (Khác)
        public string PatientBirthDate { get; set; } = string.Empty;                 // Ngày sinh theo định dạng yyyyMMdd
        public int? PatientAge { get; set; }                                          // Tuổi của bệnh nhân (tùy chọn)

        // --- Thông tin bác sĩ chỉ định ---
        public string DoctorId { get; set; } = string.Empty;                          // Mã định danh bác sĩ chỉ định
        public string DoctorName { get; set; } = string.Empty;                        // Họ tên bác sĩ chỉ định
        public string Department { get; set; } = string.Empty;                        // Khoa/phòng nơi bác sĩ công tác

        // --- Thông tin đơn hàng ---
        public string OrderId { get; set; } = string.Empty;                           // Mã đơn hàng từ hệ thống HIS (duy nhất)
        public string AccessionNumber { get; set; } = string.Empty;                   // Mã chỉ định dịch vụ gửi sang PACS
        public string StudyDescription { get; set; } = string.Empty;                  // Mô tả tổng quan yêu cầu chẩn đoán
        public DateTime ScheduledDateTime { get; set; }                               // Thời gian dự kiến thực hiện thủ thuật

        // --- Thông tin modality ---
        public string Modality { get; set; } = string.Empty;                          // Loại modality: CT, MR, CR, US, MG, XA...
        public string BodyPart { get; set; } = string.Empty;                          // Bộ phận cơ thể cần chụp
        public string ViewPosition { get; set; } = string.Empty;                      // Hướng chụp (PA, AP, LAT...)
        public string StationName { get; set; } = string.Empty;                       // Tên thiết bị chụp (dùng trong kỹ thuật)

        // --- Lịch trình chi tiết theo chuẩn DICOM MWL ---
        public string ScheduledStationAETitle { get; set; } = string.Empty;           // AE Title của máy modality thực hiện
        public string ScheduledProcedureStepStartDate { get; set; } = string.Empty;   // Ngày dự kiến thực hiện (yyyyMMdd)
        public string ScheduledProcedureStepStartTime { get; set; } = string.Empty;   // Giờ dự kiến thực hiện (HHmmss)
        public string ScheduledPerformingPhysicianName { get; set; } = string.Empty;  // Tên bác sĩ thực hiện (khác bác sĩ chỉ định)
        public string ScheduledProcedureStepDescription { get; set; } = string.Empty; // Mô tả bước thực hiện cụ thể
        public string ScheduledStationName { get; set; } = string.Empty;              // Tên thiết bị theo kỹ thuật viên sử dụng
        public string ExamRoom { get; set; } = string.Empty;                          // Phòng khám nơi thực hiện thủ thuật (nếu có)
        public string ExamDescription { get; set; } = string.Empty;                    // Mô tả chi tiết về thủ thuật (nếu có)
        public string HospitalName { get; set; } = string.Empty;                       // Tên bệnh viện (nếu có)

        // --- Thông tin thêm ---
        public string Notes { get; set; } = string.Empty;                             // Ghi chú bổ sung từ HIS (nếu có)
    }
}

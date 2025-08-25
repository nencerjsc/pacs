using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NencerApi.Modules.PacsServer.Model
{
    [Table("DicomStudies")]
    public class DicomStudyModel
    {
        [Key]
        public string StudyInstanceUID { get; set; }        // 0020000D
        public string PatientID { get; set; } = string.Empty;                 // 00100020
        public string PatientName { get; set; } = string.Empty;               // 00100010 (Alphabetic)
        public string PatientSex { get; set; } = string.Empty;                // 00100040
        public string PatientBirthDate { get; set; } = string.Empty;          // 00100030
        public string AccessionNumber { get; set; } = string.Empty;           // 00080050
        public string StudyDate { get; set; } = string.Empty;                 // 00080020
        public string StudyTime { get; set; } = string.Empty;                 // 00080030
        public string StudyID { get; set; } = string.Empty;                   // 00200010
        public string StudyDescription { get; set; } = string.Empty;              // 00081030
        public string[] ModalitiesInStudy { get; set; } = Array.Empty<string>();     // 00080061
        public int NumberOfStudyRelatedSeries { get; set; } = 0;   // 00201206
        public int NumberOfStudyRelatedInstances { get; set; } = 0; // 00201208
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

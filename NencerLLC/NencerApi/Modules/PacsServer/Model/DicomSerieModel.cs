using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NencerApi.Modules.PacsServer.Model
{
    [Table("DicomSeries")]
    public class DicomSerieModel
    {
        [Key]
        public string SeriesInstanceUID { get; set; }         // 0020000E        
        public int SeriesNumber { get; set; } = 0;                  // 00200011       
        public string Modality { get; set; } = string.Empty;                  // 00080060
        public string StudyDate { get; set; } = string.Empty;                 // 00080021
        public string StudyTime { get; set; } = string.Empty;                 // 00080031
        public string Manufacturer { get; set; } = string.Empty;              // 00080070
        public string InstitutionName { get; set; } = string.Empty;           // 00080080
        public string SeriesDescription { get; set; } = string.Empty;         // 00181030
        public string SopClass { get; set; } = string.Empty;                  // 00081090
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int NumberOfStudyRelatedInstances { get; set; } = 0; // 00201208
        public string StudyInstanceUID { get; set; }      // 0020000D
    }
}

namespace NencerApi.Modules.PacsServer.Model
{
    public class DicomSerieModel
    {
        public string SeriesInstanceUID { get; set; } = string.Empty;         // 0020000E        
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
        // Foreign key
        public string StudyInstanceUID { get; set; } = string.Empty;        // 0020000D
        public DicomStudyModel Study { get; set; } = null!;
        public ICollection<DicomInstanceModel> Instances { get; set; } = new List<DicomInstanceModel>();

        public Dictionary<string, object> ToDicomJson()
        {
            var dicomJson = new Dictionary<string, object>();

            dicomJson["0020000D"] = new
            {
                vr = "UI",
                Value = new[] { StudyInstanceUID }
            };

            dicomJson["00200011"] = new
            {
                vr = "IS",
                Value = new[] { SeriesNumber }
            };

            dicomJson["0020000E"] = new
            {
                vr = "UI",
                Value = new[] { SeriesInstanceUID }
            };

            dicomJson["00080060"] = new
            {
                vr = "CS",
                Value = new[] { Modality }
            };

            dicomJson["00080021"] = new
            {
                vr = "DA",
                Value = new[] { StudyDate }
            };

            dicomJson["00080031"] = new
            {
                vr = "TM",
                Value = new[] { StudyTime }
            };

            dicomJson["00080070"] = new
            {
                vr = "LO",
                Value = new[] { Manufacturer }
            };

            dicomJson["00080080"] = new
            {
                vr = "LO",
                Value = new[] { InstitutionName }
            };

            dicomJson["00181030"] = new
            {
                vr = "LO",
                Value = new[] { SeriesDescription }
            };

            dicomJson["00081090"] = new
            {
                vr = "LO",
                Value = new[] { SopClass }
            };

            return dicomJson;
        }
    }
}

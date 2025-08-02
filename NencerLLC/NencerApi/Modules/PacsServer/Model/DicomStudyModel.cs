namespace NencerApi.Modules.PacsServer.Model
{
    public class DicomStudyModel
    {
        public string StudyInstanceUID { get; set; } = string.Empty;          // 0020000D
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

        // Navigation Properties
        public ICollection<DicomSerieModel> Series { get; set; } = new List<DicomSerieModel>();
        public ICollection<DicomScheduledProcedureStepModel> ScheduledProcedures { get; set; } = new List<DicomScheduledProcedureStepModel>();
        public ICollection<DicomWorkListModel> WorkLists { get; set; } = new List<DicomWorkListModel>();
        public Dictionary<string, object> ToDicomJson()
        {
            var dicomJson = new Dictionary<string, object>();

            void AddTag(string tag, string vr, object value)
            {
                if (value is string s && string.IsNullOrWhiteSpace(s)) return;
                if (value is Array a && a.Length == 0) return;
                dicomJson[tag] = new { vr, Value = value };
            }

            AddTag("0020000D", "UI", new[] { StudyInstanceUID });
            AddTag("00100020", "LO", new[] { PatientID });
            if (!string.IsNullOrWhiteSpace(PatientName))
            {
                dicomJson["00100010"] = new { vr = "PN", Value = new[] { new { Alphabetic = PatientName } } };
            }
            AddTag("00100040", "CS", new[] { PatientSex });
            AddTag("00100030", "DA", new[] { PatientBirthDate });
            AddTag("00080050", "SH", new[] { AccessionNumber });
            AddTag("00080020", "DA", new[] { StudyDate });
            AddTag("00080030", "TM", new[] { StudyTime });
            AddTag("00200010", "SH", new[] { StudyID });
            AddTag("00081030", "LO", new[] { StudyDescription });
            AddTag("00080061", "CS", ModalitiesInStudy);
            AddTag("00201206", "IS", new[] { NumberOfStudyRelatedSeries });
            AddTag("00201208", "IS", new[] { NumberOfStudyRelatedInstances });


            return dicomJson;
        }
    }
}

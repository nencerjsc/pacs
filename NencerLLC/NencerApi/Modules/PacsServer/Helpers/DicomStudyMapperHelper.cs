using NencerApi.Modules.PacsServer.Model;

namespace NencerApi.Modules.PacsServer.Helpers
{
    public class DicomStudyMapperHelper
    {
        public static Dictionary<string, object> ToDicomJson(DicomStudyModel study)
        {
            var dicomJson = new Dictionary<string, object>();

            foreach (var kvp in DicomTagMapperHelper.StudyTagMap)
            {
                var tag = kvp.Key;
                var vr = kvp.Value.vr;
                var getter = kvp.Value.getter;
                var value = getter.Invoke(study);


                if (value is string s && string.IsNullOrWhiteSpace(s)) continue;
                if (value is Array a && a.Length == 0) continue;
                if (value is int i && i == 0) continue;

                // Đặc biệt với PatientName theo kiểu PN cần định dạng riêng
                if (tag == "00100010" && value is string name)
                {
                    dicomJson[tag] = new
                    {
                        vr,
                        Value = new[] {
                        new { Alphabetic = name }
                    }
                    };
                }
                else
                {
                    dicomJson[tag] = new
                    {
                        vr,
                        Value = value is Array or IEnumerable<object> ? value : new[] { value }
                    };
                }
            }

            return dicomJson;
        }
    }
}

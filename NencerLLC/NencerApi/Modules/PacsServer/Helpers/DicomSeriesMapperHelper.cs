using NencerApi.Modules.PacsServer.Model;

namespace NencerApi.Modules.PacsServer.Helpers
{
    public class DicomSeriesMapperHelper
    {
        public static Dictionary<string, object> ToDicomJson(DicomSerieModel series)
        {
            var dicomJson = new Dictionary<string, object>();

            foreach (var kvp in DicomTagMapperHelper.SeriesTagMap)
            {
                var tag = kvp.Key;
                var vr = kvp.Value.vr;
                var getter = kvp.Value.getter;

                var value = getter.Invoke(series);

                if (value is string s && string.IsNullOrWhiteSpace(s)) continue;
                if (value is Array a && a.Length == 0) continue;
                if (value is int i && i == 0) continue;

                dicomJson[tag] = new
                {
                    vr,
                    Value = value is Array or IEnumerable<object> ? value : new[] { value }
                };
            }

            return dicomJson;
        }
    }
}

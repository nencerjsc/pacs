namespace NencerApi.Modules.PacsServer.Model
{
    public class DicomTagItemModel
    {
        public int Id { get; set; }
        public string SOPInstanceUID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;        // 00100010
        public string Title { get; set; } = string.Empty;       // Patient's Name
        public string ValueType { get; set; } = string.Empty;   // PN, UI, DA, CS,...
        public string Value { get; set; } = string.Empty;       // Nguyễn Văn A
        public string Description { get; set; } = string.Empty; // Ghi chú thêm (có thể để trống)
        public string ParentTag { get; set; } = string.Empty;   // Nếu là tag con của SQ
        public int? GroupId { get; set; } = null;               // Để phân biệt nhiều item trong cùng 1 Sequence
        public List<DicomTagItemModel>? Children { get; set; } = null; // Danh sách tag con nếu VR = SQ
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Dictionary<string, object> ToDicomJson()
        {
            var dicomJson = new Dictionary<string, object>();

            if (string.IsNullOrEmpty(Name))
                return dicomJson;

            if (Name.Equals("7FE00010", StringComparison.OrdinalIgnoreCase))
            {
                dicomJson[Name] = new
                {
                    vr = ValueType,
                    BulkDataURI = Value
                };
            }
            else if (ValueType.ToUpperInvariant() == "SQ" && Children != null && Children.Any())
            {
                // Nếu là Sequence (SQ)
                var sequenceArray = new List<Dictionary<string, object>>();

                var groupedChildren = Children
                    .GroupBy(c => c.GroupId)
                    .ToList();

                foreach (var group in groupedChildren)
                {
                    var item = new Dictionary<string, object>();

                    foreach (var child in group)
                    {
                        var childJson = child.ToDicomJson();
                        foreach (var kv in childJson)
                        {
                            item[kv.Key] = kv.Value;
                        }
                    }

                    sequenceArray.Add(item);
                }

                dicomJson[Name] = new
                {
                    vr = ValueType,
                    Value = sequenceArray
                };
            }
            else
            {
                var parsedValue = ConvertValueByVR(ValueType, Value);

                dicomJson[Name] = new
                {
                    vr = ValueType,
                    Value = new[] { parsedValue }
                };
            }

            return dicomJson;
        }


        private object ConvertValueByVR(string? vr, string rawValue)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rawValue))
                    return string.Empty;

                vr = vr?.ToUpperInvariant();

                switch (vr)
                {
                    // Các loại số nguyên
                    case "IS":
                    case "SS":
                    case "SL":
                    case "UL":
                    case "US":
                        if (int.TryParse(rawValue, out var intValue))
                            return intValue;
                        break;

                    // Các loại số thực
                    case "DS":
                    case "FL":
                    case "FD":
                        {
                            // Nếu chuỗi có dấu '\', tách ra thành mảng double
                            if (rawValue.Contains("\\"))
                            {
                                var parts = rawValue.Split('\\');
                                var list = new List<double>();
                                foreach (var part in parts)
                                {
                                    if (double.TryParse(part.Trim(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d))
                                    {
                                        list.Add(d);
                                    }
                                }
                                return list;
                            }
                            else
                            {
                                // Nếu không có '\', trả 1 số thực
                                if (double.TryParse(rawValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d))
                                    return d;
                            }
                        }
                        break;

                    // Tag (AT): luôn là chuỗi dạng ggggeeee
                    case "AT":
                        return rawValue;

                    // Các loại text chuỗi
                    case "AE":
                    case "AS":
                    case "CS":
                    case "DA":
                    case "DT":
                    case "LO":
                    case "LT":
                    case "PN":
                    case "SH":
                    case "ST":
                    case "TM":
                    case "UC":
                    case "UI":
                    case "UR":
                    case "UT":
                    default:
                        return rawValue;
                }
            }
            catch
            {
                // Nếu lỗi thì fallback về string gốc
            }

            return rawValue;
        }
    }
}

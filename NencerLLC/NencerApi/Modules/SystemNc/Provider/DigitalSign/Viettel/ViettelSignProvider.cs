using NencerApi.Modules.SystemNc.Model.DigiSign;

namespace NencerApi.Modules.SystemNc.Provider.DigitalSign.Viettel
{
    public class ViettelSignProvider : IDigitalSignProvider
    {
        public string ProviderCode => "viettel";

        public byte[] SignData(byte[] data, string dataType, string? configJson = null)
        {
            dataType = dataType.ToLower();

            if (string.IsNullOrEmpty(dataType))
                throw new ArgumentException("Data type không được rỗng.");

            // Có thể truyền thêm thông tin từ configJson vào từng hàm xử lý
            return dataType switch
            {
                "pdf" => SignPdf(data, configJson),
                "xml" => SignXml(data, configJson),
                _ => throw new NotSupportedException($"Không hỗ trợ ký định dạng: {dataType}")
            };
        }

        private byte[] SignPdf(byte[] data, string? configJson)
        {
            // Gọi tới API ký PDF của Viettel
            return data;
        }

        private byte[] SignXml(byte[] data, string? configJson)
        {
            // Gọi tới API ký XML của Viettel
            return data;
        }
    }
}

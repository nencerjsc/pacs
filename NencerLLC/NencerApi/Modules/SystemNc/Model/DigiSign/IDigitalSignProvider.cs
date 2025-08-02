namespace NencerApi.Modules.SystemNc.Model.DigiSign
{
    public interface IDigitalSignProvider
    {
        public string ProviderCode { get; } // ví dụ: "viettel", "vnpt"
        public byte[] SignData(byte[] data, string dataType, string? configJson = null);
    }
}

namespace NencerApi.Modules.SystemNc.Model.DigiSign
{
    public class DigitalSignFactory
    {
        private readonly Dictionary<string, IDigitalSignProvider> _providers;

        public DigitalSignFactory(IEnumerable<IDigitalSignProvider> providers)
        {
            _providers = providers.ToDictionary(p => p.ProviderCode.ToLower(), p => p);
        }

        public IDigitalSignProvider GetProvider(string providerCode)
        {
            if (_providers.TryGetValue(providerCode.ToLower(), out var provider))
            {
                return provider;
            }

            throw new ArgumentException("Nhà cung cấp không hợp lệ hoặc chưa được hỗ trợ.");
        }
    }
}

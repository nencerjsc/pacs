namespace NencerApi.Shared
{
    public class BasePaggingFilter
    {
        public string? FilterText { get; set; }
        public int? Page { get; set; } = 1;

        public int? PageSize { get; set; } = 100;
    }
}

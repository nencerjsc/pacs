namespace NencerApi.Modules.PacsServer.Model
{
    public class StoragePathModel
    {
        public int Id { get; set; }
        public string Path { get; set; } = null!;
        public int Priority { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

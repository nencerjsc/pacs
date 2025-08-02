namespace NencerApi.Modules.User.Model
{
    public class UserDto
    {
        public long Id { get; set; }

        public string Name { get; set; } = null!;
        public string? UserName { get; set; }
    }
}

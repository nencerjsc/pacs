namespace NencerApi.Modules.User.Model
{
    public class UserInfoAndRoles
    {
        public UserDto UserDto { get; set; }
        public List<Role>? ListRole { get; set; }
    }
}

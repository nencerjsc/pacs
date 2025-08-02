namespace NencerApi.Modules.User.Model
{
    public class UserAndRolesReq
    {
        public int UserId { get; set; }
        public List<int> ListRoleId { get; set; }
    }
}

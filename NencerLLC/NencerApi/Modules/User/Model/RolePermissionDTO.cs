namespace NencerApi.Modules.User.Model
{
    public class RolePermissionDTO
    {
        public UserModel User { get; set; } // Thông tin người dùng
        public List<string>? Roles { get; set; } // Danh sách các vai trò của người dùng
        public List<string>? Permissions { get; set; } // Danh sách các quyền của người dùng
        public Dictionary<string, bool>? DictPermissions { get; set; }

        public List<int>? RoomIds { get; set; }
        public bool? AllowAllRoom { get; set; }
    }
}

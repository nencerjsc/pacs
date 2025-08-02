using NencerApi.Modules.User.Model;

namespace NencerApi.Modules.User.Model
{
    public class RoleWithPermissionsDto
    {
        public int? RoleId { get; set; }
        public string RoleName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public List<RolePermission>? ListRolePermission { get; set; }
    }
}

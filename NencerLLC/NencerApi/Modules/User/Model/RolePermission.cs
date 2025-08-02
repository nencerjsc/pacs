using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace NencerApi.Modules.User.Model
{
    [Table("user_role_permissions")]
    public class RolePermission
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("role_id")]
        public int RoleId { get; set; }

        [Column("permission_id")]
        public int PermissionId { get; set; }

        [Column("is_active")]
        public bool? IsActive { get; set; } = true;
    }
}

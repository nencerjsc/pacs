using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NencerApi.Modules.User.Model
{
    [Table("user_has_roles")]
    public class UserHasRoles
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("role_id")]
        public int RoleId { get; set; }

        [Column("is_active")]
        public bool? IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}

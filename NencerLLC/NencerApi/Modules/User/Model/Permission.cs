using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NencerApi.Modules.User.Model
{
    [Table("user_permissions")]
    public class Permission
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        [Required]
        public string? Name { get; set; }

        [Column("module")]
        public string? ModuleName { get; set; }

        [Column("route")]
        public string? Route { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        //public ICollection<RolePermission> RolePermissions { get; set; }

        [Column("type")]
        public string? Type { get; set; }

        [Column("parent_id")]
        public int? ParentId { get; set; }

        [Column("is_active")]
        public bool? IsActive { get; set; } = true;

    }
}

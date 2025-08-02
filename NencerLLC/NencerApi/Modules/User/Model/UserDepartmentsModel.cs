using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NencerApi.Modules.User.Model
{
    [Table("user_departments")]
    public class UserDepartmentsModel
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("name_in")]
        public string? NameIn { get; set; }

        [Column("name_search")]
        public string? NameSearch { get; set; }

        [Column("code")]
        public string? Code { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("description_in")]
        public string? DescriptionIn { get; set; }

        [Column("manager_id")]
        public int? ManagerId { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}

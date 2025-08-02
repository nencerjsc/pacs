using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NencerApi.Modules.SystemNc.Model
{
    [Table("settings")]
    public class Setting
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("key")]
        [Required]
        public string Key { get; set; }
        [Column("value")]
        public string? Value { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("group")]
        public string? Group { get; set; }
    }
}

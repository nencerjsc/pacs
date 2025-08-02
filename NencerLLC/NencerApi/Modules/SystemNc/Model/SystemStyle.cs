using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NencerApi.Modules.SystemNc.Model
{
    [Table("system_styles")]
    public class SystemStyle
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("key")]
        [Required]
        public string Code { get; set; }

        [Column("note")]
        public string? Value { get; set; }
    }
}

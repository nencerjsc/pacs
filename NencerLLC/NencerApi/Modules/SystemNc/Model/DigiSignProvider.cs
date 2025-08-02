using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NencerApi.Modules.SystemNc.Model
{
    [Table("digisign_providers")]
    public class DigiSignProvider
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        [MaxLength(50)]
        public string? Name { get; set; }

        [Column("code")]
        [MaxLength(50)]
        public string? Code { get; set; }

        [Column("config_json")]
        public string? ConfigJson { get; set; }

        [Column("status")]
        public int? Status { get; set; }

        [Column("sort")]
        public int? Sort { get; set; }
    }
}

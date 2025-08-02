using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NencerApi.Modules.SystemNc.Model
{
    [Table("digisign_logs")]
    public class DigiSignLog
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }  // BIGINT → long

        [Column("provider")]
        [MaxLength(50)]
        public string? Provider { get; set; }

        [Column("logs")]
        public string? Logs { get; set; }

        [Column("filename")]
        [MaxLength(250)]
        public string? FileName { get; set; }

        [Column("user_id")]
        public int? UserId { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}

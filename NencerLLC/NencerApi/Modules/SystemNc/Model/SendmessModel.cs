using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NencerApi.Modules.SystemNc.Model
{
    [Table("sendmess")]
    public class SendmessModel
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        [MaxLength(50)]
        public string? Name { get; set; }

        [Column("type")]
        [MaxLength(50)]
        public string? Type { get; set; }

        [Column("provider")]
        [MaxLength(50)]
        public string? Provider { get; set; }

        [Column("url")]
        [MaxLength(250)]
        public string? Url { get; set; }

        [Column("port")]
        [MaxLength(10)]
        public string? Port { get; set; }

        [Column("api_key")]
        [MaxLength(250)]
        public string? ApiKey { get; set; }

        [Column("api_secret")]
        [MaxLength(250)]
        public string? ApiSecret { get; set; }

        [Column("configs")]
        public string? Configs { get; set; }

        [Column("status")]
        public int? Status { get; set; } = 1;

        [Column("sort")]
        public int? Sort { get; set; }

        [Column("daily_send")]
        public int? DailySend { get; set; }

        [Column("mail_path")]
        [MaxLength(50)]
        public string? MailPath { get; set; }
    }
}

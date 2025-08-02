using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NencerApi.Modules.SystemNc.Model
{
    [Table("file_servers")]
    public class FileServer
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("server_name")]
        [MaxLength(50)]
        public string ServerName { get; set; }

        [Column("type")]
        [MaxLength(50)]
        public string? Type { get; set; } = "FTP";

        [Column("host_name")]
        [MaxLength(100)]
        public string? HostName { get; set; }

        [Column("user_name")]
        [MaxLength(50)]
        public string? UserName { get; set; }

        [Column("password")]
        [MaxLength(100)]
        public string? Password { get; set; }

        [Column("port")]
        [MaxLength(10)]
        public string? Port { get; set; }

        [Column("folder")]
        [MaxLength(100)]
        public string? Folder { get; set; }

        [Column("api_key")]
        [MaxLength(150)]
        public string? ApiKey { get; set; }

        [Column("api_secret")]
        [MaxLength(150)]
        public string? ApiSecret { get; set; }

        [Column("status")]
        public int? Status { get; set; } = 1;

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}

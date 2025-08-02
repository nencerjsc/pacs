using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NencerApi.Modules.SystemNc.Model
{
    [Table("currencies")]
    public class CurrencyModel
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string? Name { get; set; }

        [Column("code")]
        [Required]
        public string? Code { get; set; }

        [Column("value")]
        public decimal? Value { get; set; }

        [Column("decimal")]
        public int Decimal { get; set; } = 0;

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}

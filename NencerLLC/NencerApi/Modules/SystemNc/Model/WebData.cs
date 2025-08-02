using System.ComponentModel.DataAnnotations.Schema;
namespace NencerApi.Modules.SystemNc.Model
{
    [Table("web_datas")]
    public class WebData
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
        public int? Status { get; set; }
        [Column("created_at")] public DateTime? CreatedAt { get; set; }
        [Column("updated_at")] public DateTime? UpdatedAt { get; set; }
    }
}

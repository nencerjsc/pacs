using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NencerApi.Modules.SystemNc.Model
{
    [Table("menu_first")]
    public class MenuFirst
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        [Required]
        public string Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("image")]
        public string? Image { get; set; }

        [Column("image_bg")]
        public string? ImageBg { get; set; }

        [Column("fa_icon")]
        public string? FaIcon { get; set; }

        [Column("type_id")]
        public int? TypeId { get; set; }

        [Column("url")]
        public string? Url { get; set; }

        [Column("sort")]
        public int? Sort { get; set; }

        [Column("status")]
        public int? Status { get; set; } = 1;

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

    }


    public class MenuFirstUpdate
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? ImageBg { get; set; }
        public string? FaIcon { get; set; }
        public int? TypeId { get; set; }
        public string? Url { get; set; }
        public int? Sort { get; set; }
        public int? Status { get; set; } = 1;
    }

    public class MenuFirstDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? ImageBg { get; set; }
        public string? FaIcon { get; set; }
        public string? Url { get; set; }
        public int? Sort { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<MenuSecondDto> MenuSeconds { get; set; }
    }

    public class MenuSecondDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? FaIcon { get; set; }
        public string? Url { get; set; }
        public int? Sort { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }


}

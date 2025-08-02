using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NencerApi.Modules.User.Model
{
    [Table("user_job_reviews")]
    public class UserJobReviewsModel
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("review_date")]
        public DateTime? ReviewDate { get; set; }

        [Column("review_score")]
        public decimal ReviewScore { get; set; }

        [Column("comments")]
        public string? Comments { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}

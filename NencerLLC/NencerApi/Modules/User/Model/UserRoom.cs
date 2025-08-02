using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NencerApi.Modules.User.Model
{
    [Table("user_room")]
    public class UserRoom
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("room_id")]
        public int RoomId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}

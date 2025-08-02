using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NencerApi.Modules.User.Model
{
    [Table("users")]
    public class UserModel
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        [Required]
        public string Name { get; set; }

        [Column("username")]
        [Required]
        [StringLength(100, ErrorMessage = "Username cannot be longer than 100 characters.")]
        public string Username { get; set; }

        [Column("department_id")]
        public int? DepartmentId { get; set; }

        [NotMapped]
        public string? DepartmentName { get; set; }

        [Column("password")]
        [Required]
        [StringLength(255, ErrorMessage = "Password must be between 6 and 255 characters.", MinimumLength = 6)]
        public string Password { get; set; }

        [Column("email")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }

        [Column("phone")]
        [RegularExpression(@"^0\d{0,10}$", ErrorMessage = "Phone number must start with 0 and be up to 11 digits.")]
        public string? Phone { get; set; }
        [Column("tmp_token")]
        public string? TmpToken { get; set; }
        [Column("tmp_expiration")]
        public DateTime? TmpExpiration { get; set; }
        [Column("signature")]
        public string? Signature { get; set; } //base64 lưu ảnh chữ kí

        [Column("is_active")]
        public bool? IsActive { get; set; }

        [Column("allow_all_room")]
        public bool? AllowAllRoom { get; set; }

    }
}

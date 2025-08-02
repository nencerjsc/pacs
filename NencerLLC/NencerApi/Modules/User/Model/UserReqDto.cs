using System.ComponentModel.DataAnnotations;

namespace NencerApi.Modules.User.Model
{
    public class UserReqDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? TmpToken { get; set; }

        public DateTime? TmpExpiration { get; set; }

        public string? Signature { get; set; } //base64 lưu ảnh chữ kí

        public bool? IsActive { get; set; }

        public List<int>? RoleIds { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace NencerApi.Modules.User.Model
{
    public class AuthRequest
    {
        [Required(ErrorMessage = "username_is_required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "password_is_required")]
        public string Password { get; set; }
    }
}

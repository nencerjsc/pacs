namespace NencerApi.Helpers
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, 12);
        }

        // Sử dụng BCrypt để kiểm tra xem mật khẩu nhập vào có khớp với mật khẩu đã được mã hóa không
        public static bool VerifyPassword(string text, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(text, hash);
        }
    }
}

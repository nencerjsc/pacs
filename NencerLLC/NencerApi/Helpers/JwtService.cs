using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.IdentityModel.Tokens;
using NencerApi.Modules.User.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NencerApi.Helpers
{
    public class JwtService
    {

        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(UserModel user, IList<string> roles, IList<string> permissions)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username)
        };

            // Thêm vai trò và quyền vào claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            foreach (var permission in permissions)
            {
                claims.Add(new Claim("Permission", permission));
            }

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(1440),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //public string GenerateToken(UserModel user , List<string>? permissons = null)
        //{
        //    try
        //    {
        //        int.TryParse(_config["Jwt:ExpiryMinutes"], out int expiryMinutes);
        //        var tokenHandler = new JwtSecurityTokenHandler();
        //        var secretKey = _config["Jwt:Key"];
        //        var secretByte = Encoding.UTF8.GetBytes(secretKey);
        //        var tokenDescription = new SecurityTokenDescriptor
        //        {
        //            Subject = new System.Security.Claims.ClaimsIdentity(new[]
        //            {

        //             new Claim("Username", user.Username),
        //             new Claim("Id", user.Id.ToString()),
        //             new Claim("TokenId", Guid.NewGuid().ToString()),
        //         }),
        //            Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
        //            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretByte), SecurityAlgorithms.HmacSha256Signature)
        //        };

        //        var token = tokenHandler.CreateToken(tokenDescription);
        //        return tokenHandler.WriteToken(token);
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }

        //}

        public string GenerateToken(UserModel user, List<string>? roles = null, bool? allowAllRoom = false, List<int>? roomIds = null)
        {
            try
            {
                // Lấy thời gian hết hạn từ cấu hình
                int.TryParse(_config["Jwt:ExpiryMinutes"], out int expiryMinutes);

                // Tạo token handler và key
                var tokenHandler = new JwtSecurityTokenHandler();
                var secretKey = _config["Jwt:Key"];
                var secretByte = Encoding.UTF8.GetBytes(secretKey);

                // Danh sách các claims cơ bản
                var claims = new List<Claim>
                    {
                        new Claim("Username", user.Username),
                        new Claim("Id", user.Id.ToString()),
                        new Claim("TokenId", Guid.NewGuid().ToString())
                    };

                // Thêm roles vào claims
                if (roles != null && roles.Any())
                {
                    claims.AddRange(roles.Select(role => new Claim("Role", role)));
                }

                // Thêm roomIds vào claims nếu có
                string roomIdsString = "";
                if (allowAllRoom == true)
                {
                    roomIdsString = "all";
                }
                else if (roomIds != null && roomIds.Any())
                {
                    roomIdsString = String.Join(",", roomIds);  // Chuyển đổi danh sách ID thành một chuỗi
                }
                claims.Add(new Claim("RoomIds", roomIdsString));

                // Tạo mô tả token
                var tokenDescription = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretByte), SecurityAlgorithms.HmacSha256Signature)
                };

                // Tạo token và trả về
                var token = tokenHandler.CreateToken(tokenDescription);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                return null;
            }
        }

        public string GenerateRefreshToken(UserModel user, List<string>? roles = null, bool? allowAllRoom = false, List<int>? roomIds = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var claims = new List<Claim>
                {
                    new Claim("TokenId", Guid.NewGuid().ToString()),
                    new Claim("Id", user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
                };

            // Thêm permissions vào claims nếu có
            if (roles != null && roles.Any())
            {
                claims.AddRange(roles.Select(role => new Claim("Role", role)));
            }

            // Thêm roomIds vào claims nếu có
            string roomIdsString = "";
            if (allowAllRoom == true)
            {
                roomIdsString = "all";
            }
            else if (roomIds != null && roomIds.Any())
            {
                roomIdsString = String.Join(",", roomIds);  // Chuyển đổi danh sách ID thành một chuỗi
            }
            claims.Add(new Claim("RoomIds", roomIdsString));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7), // Refresh Token sống 7 ngày
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public int? ValidateRefreshToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

            // Xác minh Refresh Token
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            };

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

            if (validatedToken is JwtSecurityToken jwtToken &&
                jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                int.TryParse(principal.Claims.FirstOrDefault(s => s.Type == "Id")?.Value, out int UserId);
                return UserId;
            }
            return null;
        }
    }
}

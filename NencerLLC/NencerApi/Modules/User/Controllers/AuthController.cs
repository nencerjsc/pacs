using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NencerApi;
using NencerApi.Helpers;
using NencerApi.Modules.SystemNc.Model;
using NencerApi.Modules.User.Model;

namespace NencerCore.Modules.User.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly JwtService _jwtService;
        private readonly UserService _userService;

        public AuthController(AppDbContext context, IConfiguration configuration, JwtService jwtService, UserService userService)
        {
            _context = context;
            _configuration = configuration;
            _jwtService = jwtService;
            _userService = userService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {

            var res = await _userService.Authenticate(login.Username, login.Password);
            if (res == null || res.User == null)
            {
                return Unauthorized(new BaseResponse<UserModel>("404", "invalid_credentials", null));
            }

            //var token = _jwtService.GenerateToken(res.User, res.Roles, res.Permissions);
            var token = _jwtService.GenerateToken(res.User, res.Roles , res.AllowAllRoom , res.RoomIds);

            if (token == null)
            {
                return Unauthorized(new BaseResponse<UserModel>("404", "invalid_credentials", null));
            }
            else
            {
                // Tạo Refresh Token
                var refreshToken = _jwtService.GenerateRefreshToken(res.User, res.Roles, res.AllowAllRoom, res.RoomIds);
                var data = new { Token = token, Info = res, RefreshToken = refreshToken };
                return Ok(new BaseResponse<Object>("200", "success", data));
            }

        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(RefreshTokenDto rq)
        {
            try
            {
                var UserId = _jwtService.ValidateRefreshToken(rq.RefreshToken);
                if (UserId > 0)
                {
                    var User = _context.Users.FirstOrDefault(x => x.Id == UserId);
                    if (User != null)
                    {
                        var token = _jwtService.GenerateToken(User);

                        if (token == null)
                        {
                            return Unauthorized(new BaseResponse<UserModel>("404", "invalid_credentials", null));
                        }
                        else
                        {
                            // Tạo Refresh Token
                            var refreshToken = _jwtService.GenerateRefreshToken(User);
                            var roleIds = _context.UserHasRoles.Where(r => r.UserId == User.Id).Select(r => r.RoleId).ToArray();
                            var permissionIds = _context.RolePermissions.Where(r => roleIds.Contains(r.RoleId)).Select(r => r.PermissionId).ToArray();
                            var permissions = _context.Permissions
                                .Where(p => permissionIds.Contains(p.Id))
                                .Select(p => p.Name)
                                .ToList();

                            var roles = _context.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Name).ToList();
                            var info = new RolePermissionDTO
                            {
                                User = User,
                                Roles = roles,
                                Permissions = permissions,
                                //DictPermissions = await _permissionService.GetDictPermissions(user.Id)
                            };
                            var data = new { Token = token, Info = info, RefreshToken = refreshToken };
                            return Ok(new BaseResponse<Object>("200", "success", data));
                        }
                    }
                }
                return Unauthorized(new BaseResponse<UserModel>("404", "invalid_refresh_token", null));
            }
            catch (Exception ex)
            {
                return Unauthorized(new BaseResponse<UserModel>("404", "exception: " + ex.Message, null));

            }

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserModel user)
        {

            if (_context.Users.Any(u => u.Username == user.Username))
            {
                return BadRequest(new BaseResponse<UserModel>("404", "user_name_existed", null));
            }

            if (_context.Users.Any(u => u.Phone == user.Phone))
            {
                return BadRequest(new BaseResponse<UserModel>("404", "phone_existed", null));
            }

            if (_context.Users.Any(u => u.Email == user.Email))
            {
                return BadRequest(new BaseResponse<UserModel>("404", "email_existed", null));
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password, 10);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new BaseResponse<UserModel>("200", "register_success", user));
        }

        private string GenerateJwtToken(UserModel user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            }),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpiryMinutes"])),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}

using Microsoft.EntityFrameworkCore;
using NencerApi.Modules.User.Model;
using NencerApi.Modules.User.Service;
using NencerCore;

namespace NencerApi.Helpers
{
    public class UserService
    {

        private readonly AppDbContext _context;
        private PermissionService _permissionService;

        public UserService(AppDbContext context, PermissionService permissionService)
        {
            _context = context;
            _permissionService = permissionService;
        }

        public async Task<RolePermissionDTO> Authenticate(string username, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null || !VerifyPassword(password, user.Password))
                return null;

            user.Signature = null;
            user.Password = null;
            // get role
            // get permision
            // get role permiss
            // add vao 1 list 
            //phair timf has role truoc
            var roleIds = _context.UserHasRoles
                .Where(r => r.UserId == user.Id && r.IsActive == true)
                .Select(r => r.RoleId).ToArray();

            var activeRoleIds = _context.Roles
                .Where(x => x.IsActive == true && roleIds.Contains(x.Id))
                .Select(x => x.Id).ToList();

            var permissionIds = _context.RolePermissions
                .Where(r => activeRoleIds.Contains(r.RoleId) && r.IsActive == true)
                .Select(r => r.PermissionId).ToArray();

            //chỉ trả về quyền frontend (để check ẩn hiện page) cho nhẹ , quyền backend sẽ check trên api
            var permissions = _context.Permissions
                .Where(p => permissionIds.Contains(p.Id) && p.Type == "frontend")
                .Select(p => p.Name)
                .ToList();

            var roles = _context.Roles.Where(r => activeRoleIds.Contains(r.Id)).Select(r => r.Name).ToList();


            return new RolePermissionDTO
            {
                User = user,
                Roles = roles,
                Permissions = permissions
            };
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            // Sử dụng BCrypt để kiểm tra xem mật khẩu nhập vào có khớp với mật khẩu đã được mã hóa không
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

    }
}

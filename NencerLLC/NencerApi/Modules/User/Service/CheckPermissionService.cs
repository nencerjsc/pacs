using Microsoft.EntityFrameworkCore;
using NencerCore;

namespace NencerApi.Modules.User.Service
{

    /// <summary>
    /// tạm thời check trên db , sau sẽ dùng Redis
    /// </summary>
    public class CheckPermissionService
    {
        private readonly AppDbContext _context;
        public CheckPermissionService(AppDbContext context) { _context = context; }


        public async Task<bool> HasPermission(string perName, List<string>? roleNames)
        {
            if (roleNames == null || roleNames.Any())
            {
                return false;
            }

            var roleIds = await _context.Roles
                .Where(x => roleNames.Contains(x.Name) && x.IsActive == true)
                .Select(x => x.Id).ToListAsync();

            var permissionIds = _context.RolePermissions
                .Where(x => roleIds.Contains(x.Id) && x.IsActive == true)
                .Select(x => x.PermissionId).ToList();

            return _context.Permissions
                .Any(x => x.IsActive == true && permissionIds.Contains(x.Id)
                        && x.Name == perName);
        }
    }
}

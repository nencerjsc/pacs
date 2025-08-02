using Microsoft.EntityFrameworkCore;
using NencerApi.Helpers;
using NencerApi.Modules.User.Model;
using NencerCore;
using System.Linq;

namespace NencerApi.Modules.User.Service
{
    public class RoleService
    {
        private readonly AppDbContext _context;

        public RoleService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Role> GetById(long? id)
        {
            return await _context.Roles.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Role>> GetAllRolesToList(bool? isActive)
        {
            return await _context.Roles
                .Where(x => !isActive.HasValue || x.IsActive == isActive)
                .ToListAsync();
        }

        public async Task<BaseResponse<List<Role>>> GetAllRoles(bool? isActive = null)
        {
            try
            {
                var data = await _context.Roles
                    .Where(x => !isActive.HasValue || x.IsActive == isActive)
                    .ToListAsync();
                return new BaseResponse<List<Role>> { Data = data };
            }
            catch (Exception ex)
            {
                return new ExceptionErrorResponse<List<Role>>(ex, "GetAllRoles");
            }
        }

        public async Task<BaseResponse<Role>> GetRoleById(int? id)
        {
            try
            {
                var data = await _context.Roles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (data == null)
                {
                    return new ErrorResponse<Role>("role_not_found");
                }
                return new BaseResponse<Role> { Data = data };
            }
            catch (Exception ex)
            {
                return new ExceptionErrorResponse<Role>(ex, "GetRoleById");
            }
        }

        public async Task<BaseResponse<object?>> CreateRoleWithPermissions(RoleWithPermissionsDto req)
        {
            using (var trans = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    //check rolename không trùng
                    var check = _context.Roles.Any(x => x.Name.ToLower() == req.RoleName.ToLower());
                    if (check) return new ErrorResponse<object?>("role_name_used");

                    var role = new Role();
                    role.Name = req.RoleName;
                    role.Description = req.Description;
                    //role.Level = 1;
                    role.IsActive = true;
                    role.CreatedAt = DateTime.Now;
                    role.UpdatedAt = DateTime.Now;

                    _context.Add(role);
                    _context.SaveChanges();

                    //list permission
                    var inputs = req.ListRolePermission;
                    if (inputs != null && inputs.Count > 0)
                    {
                        inputs.ForEach(x => x.RoleId = role.Id);
                        _context.AddRange(inputs);
                    }

                    await _context.SaveChangesAsync();
                    await trans.CommitAsync();

                    return new BaseResponse<object?>();

                }
                catch (Exception ex)
                {
                    await trans.RollbackAsync();
                    return new ExceptionErrorResponse<object?>(ex, "UpdateRoleWithPermissions");
                }
            }
        }

        public async Task<BaseResponse<object?>> UpdateRoleWithPermissions(RoleWithPermissionsDto req)
        {
            using (var trans = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (!req.RoleId.HasValue) return new ErrorResponse<object?>("Bad request");

                    // Kiểm tra tồn tại role
                    var role = await _context.Roles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.RoleId);
                    if (role == null) return new ErrorResponse<object?>("role_not_found");

                    // Cập nhật thông tin role
                    role.Name = req.RoleName;
                    role.Description = req.Description;
                    role.UpdatedAt = DateTime.Now;
                    role.IsActive = req.IsActive;

                    _context.Update(role);

                    // Danh sách permissions mới từ request
                    var inputs = req.ListRolePermission ?? new List<RolePermission>();

                    // Lấy danh sách permissions hiện tại của role
                    var oldRole_Permiss = (await GetListRolePermissionByRoleId(req.RoleId.Value)).Data;

                    // Tìm permissions cần xóa (bị uncheck)
                    var removeRole_Permiss = oldRole_Permiss.Where(x => !inputs.Any(y => y.PermissionId == x.PermissionId)).ToList();
                    _context.RemoveRange(removeRole_Permiss);

                    // Tìm permissions mới cần thêm
                    var addedPermissons = inputs.Where(x => !oldRole_Permiss.Any(y => y.PermissionId == x.PermissionId)).ToList();
                    foreach (var permission in addedPermissons)
                    {
                        permission.RoleId = (int)req.RoleId.Value;
                        permission.CreatedAt = DateTime.Now;
                        permission.UpdatedAt = DateTime.Now;
                        permission.IsActive = true;
                    }
                    _context.AddRange(addedPermissons);

                    // Lưu thay đổi
                    await _context.SaveChangesAsync();
                    await trans.CommitAsync();

                    return new BaseResponse<object?>();
                }
                catch (Exception ex)
                {
                    await trans.RollbackAsync();
                    return new ExceptionErrorResponse<object?>(ex, "UpdateRoleWithPermissions");
                }
            }
        }


        public async Task<BaseResponse<List<RolePermission>?>> GetListRolePermissionByRoleId(int roleId)
        {
            try
            {
                var listPermissions = await _context.RolePermissions.
                                     Where(s => s.RoleId == roleId && s.IsActive == true).ToListAsync();
                return new BaseResponse<List<RolePermission>?> { Data = listPermissions };
            }
            catch (Exception ex)
            {
                return new ExceptionErrorResponse<List<RolePermission>?>(ex, "GetListRolePermissionByRoleId");
            }
        }

        public async Task<BaseResponse<UserInfoAndRoles>> GetUserInfoAndRolesByUserId(int id)
        {
            try
            {
                //user
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (user == null)
                {
                    return new ErrorResponse<UserInfoAndRoles>("user_not_found");
                }
                var userDto = new UserDto { Id = id, UserName = user.Username, Name = user.Name };

                //roles
                var listRole = await GetListRoleByUserId(id);

                var data = new UserInfoAndRoles { UserDto = userDto, ListRole = listRole };
                return new BaseResponse<UserInfoAndRoles> { Data = data };
            }
            catch (Exception ex)
            {
                return new ExceptionErrorResponse<UserInfoAndRoles>(ex, "GetUserInfoAndRolesByUserId");
            }
        }

        public async Task<List<Role>> GetListRoleByUserId(int userId)
        {
            return await (from role in _context.Roles
                          join userRole in _context.UserHasRoles on role.Id equals userRole.RoleId into ps
                          from userRole in ps.DefaultIfEmpty()
                          where userRole.UserId == userId && userRole.IsActive == true && role.IsActive == true
                          select new Role()
                          {
                              Id = role.Id,
                              Name = role.Name,
                              //Level = role.Level,
                              IsActive = role.IsActive,
                          }).ToListAsync();
            ;
        }

        public async Task<BaseResponse<object?>> UpdateRolesForUser(UserAndRolesReq req)
        {
            try
            {
                var userId = req.UserId;
                var listRoleIdReq = req.ListRoleId;

                //lấy all danh sách role đang lưu của user , bao gồm cả is_status = 0 vs 1
                var allUserRole = await _context.UserHasRoles.AsNoTracking().Where(x => x.UserId == userId).ToListAsync();
                var allRoleId = allUserRole.Select(x => x.RoleId).ToList();
                if (allRoleId == null) allRoleId = new();

                //list role mới thêm -> thêm mới , trạng thái là active
                var plusRoleIds = listRoleIdReq.Except(allRoleId).ToList();
                var listPlusUserRole = new List<UserHasRoles>();
                foreach (var id in plusRoleIds)
                {
                    var item = new UserHasRoles
                    {
                        UserId = userId,
                        RoleId = id,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    listPlusUserRole.Add(item);
                }
                _context.AddRange(listPlusUserRole);

                //list in active -> tắt active nếu đang bật
                var inActiveRoleIds = allRoleId.Except(listRoleIdReq).ToList();
                var inActiveList = allUserRole.Where(x => inActiveRoleIds.Contains(x.RoleId) && x.IsActive == true).ToList();
                foreach (var item in inActiveList)
                {
                    item.IsActive = false;
                    item.UpdatedAt = DateTime.Now;
                }
                _context.UpdateRange(inActiveList);

                //list chung nhau -> active lại nếu đang bị tắt active
                var joinRoleIds = listRoleIdReq.Intersect(allRoleId).ToList();
                var activeList = allUserRole.Where(x => joinRoleIds.Contains(x.RoleId) && x.IsActive != true).ToList();
                foreach (var item in activeList)
                {
                    item.IsActive = true;
                    item.UpdatedAt = DateTime.Now;
                }
                _context.UpdateRange(activeList);

                await _context.SaveChangesAsync();

                return new BaseResponse<object?>();

            }
            catch (Exception ex)
            {
                return new ExceptionErrorResponse<object?>(ex, "UpdateRolesForUser");
            }
        }
    }
}

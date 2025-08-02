using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using NencerApi.Extentions;
using NencerApi.Helpers;
using NencerApi.Modules.User.Model;
using NencerCore;
using System.Reflection;

namespace NencerApi.Modules.User.Service
{
    public class PermissionService
    {
        private readonly AppDbContext _context;
        private readonly IApiDescriptionGroupCollectionProvider _apiProvider;

        public PermissionService(AppDbContext context, IApiDescriptionGroupCollectionProvider apiProvider)
        {
            _context = context;
            _apiProvider = apiProvider;
        }

        #region --Init permission ---
        public async Task<BaseResponse<object?>> InitPermission()
        {
            using (var trans = _context.Database.BeginTransaction())
            {
                try
                {
                    //lấy danh sách permission từ class PermissionDefined
                    var listDictPermissions = PermissionExtension.LoadPermissionsTree();
                    ProcessData(listDictPermissions);

                    await trans.CommitAsync();
                    return new BaseResponse<object?>();
                }
                catch (Exception ex)
                {
                    await trans.RollbackAsync();
                    return new ExceptionErrorResponse<object?>(ex, "InitPermission");
                }
            }
        }

        private void ProcessData(Dictionary<string, object> data)
        {
            foreach (var key in data.Keys)
            {
                //Lấy tên quyền gốc
                var root = AddPermission(key);

                var value = data[key];

                SavePermission(root, value);
            }
        }

        private void SavePermission(Permission? per, object? value)
        {
            if (value == null) return;

            // Kiểm tra và xử lý dựa trên kiểu của value
            if (value is string)
            {
                AddChild(per, (string)value);
                return;
            }
            else if (value is Dictionary<string, object>)
            {
                var dict = (Dictionary<string, object>)value;

                if (dict != null)
                {
                    foreach (var key in dict.Keys)
                    {
                        //Lấy tên quyền gốc
                        var rootChild = AddPermissionChild(per, key);

                        var valueChild = dict[key];

                        SavePermission(rootChild, valueChild);
                    }
                    return;
                }
            }
            else if (value is List<object>)
            {
                var list = (List<object>)value;

                if (list != null)
                {
                    foreach (var item in list)
                    {
                        SavePermission(per, item);
                    }
                }
                return;

            }
        }

        public Permission? AddPermission(string fieldName)
        {
            // Kiểm tra xem đã tồn tại Permission có Name là fieldName chưa
            var existingPermission = _context.Permissions.FirstOrDefault(p => p.Name == fieldName && p.IsActive == true);

            if (existingPermission != null)
            {
                // Nếu đã tồn tại -> update lại description
                var oldDesc = existingPermission.Description;
                var newDesc = PermissionExtension.GetDescriptionByValue(fieldName) ?? fieldName;
                if (oldDesc != newDesc)
                {
                    existingPermission.Description = newDesc;
                    existingPermission.UpdatedAt = DateTime.Now;
                    _context.Update(existingPermission);
                    _context.SaveChanges();
                }

                // Trả về ID của Permission đã tồn tại
                return existingPermission;
            }


            var record = _context.Permissions.Add(new Permission
            {
                Name = fieldName,
                Type = PermissionTypeEnum.ROOT.GetDisplayName(), // Đặt giá trị type tùy thuộc vào yêu cầu của bạn
                Description = PermissionExtension.GetDescriptionByValue(fieldName) ?? fieldName, // Đặt mô tả nếu có
                IsActive = true, // Đặt giá trị Status tùy thuộc vào yêu cầu của bạn
                ParentId = null, // Lấy giá trị Menu tùy thuộc vào trường field
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            });


            _context.Add(record.Entity);
            _context.SaveChanges();

            return record.Entity;
        }


        public Permission? AddChild(Permission? permission, string fieldName)
        {
            // Kiểm tra xem đã tồn tại Permission có Name là fieldName chưa
            var existingChild = _context.Permissions.FirstOrDefault(p => p.Name == fieldName && p.ParentId == permission.Id && p.IsActive == true);

            if (existingChild != null)
            {
                // Nếu đã tồn tại -> update lại description
                var oldDesc = existingChild.Description;
                var newDesc = PermissionExtension.GetDescriptionByValue(fieldName);
                if (oldDesc != newDesc)
                {
                    existingChild.Description = newDesc;
                    existingChild.UpdatedAt = DateTime.Now;
                    _context.Update(existingChild);
                    _context.SaveChanges();
                }

                // Nếu đã tồn tại, trả về ID của Permission đã tồn tại
                return permission;
            }

            //var permissionChild = _dbContext.Permissions.FirstOrDefault(p => p.Name == fieldName && p.Menu == permission.Id && p.Status == 1);

            var record = _context.Permissions.Add(new Permission
            {
                Name = fieldName,
                Type = PermissionTypeEnum.CHILD.GetDisplayName(), // Đặt giá trị type tùy thuộc vào yêu cầu của bạn
                Description = PermissionExtension.GetDescriptionByValue(fieldName), // Đặt mô tả nếu có
                IsActive = true, // Đặt giá trị Status tùy thuộc vào yêu cầu của bạn
                ParentId = (int)permission.Id, // Lấy giá trị Menu tùy thuộc vào trường field
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            });

            _context.Add(record.Entity);
            _context.SaveChanges();

            return permission;
        }
        public Permission? AddPermissionChild(Permission? per, string fieldName)
        {

            // Kiểm tra xem đã tồn tại Permission có Name là fieldName chưa và có menu chưa
            var existingPermission = _context.Permissions.FirstOrDefault(p => p.ParentId == per.Id && p.Name == fieldName && p.IsActive == true);

            if (existingPermission != null)
            {
                // Nếu đã tồn tại -> update lại description
                var oldDesc = existingPermission.Description;
                var newDesc = PermissionExtension.GetDescriptionByValue(fieldName);
                if (oldDesc != newDesc)
                {
                    existingPermission.Description = newDesc;
                    existingPermission.UpdatedAt = DateTime.Now;
                    _context.Update(existingPermission);
                    _context.SaveChanges();
                }

                // Nếu đã tồn tại, trả về ID của Permission đã tồn tại
                return existingPermission;
            }


            var record = _context.Permissions.Add(new Permission
            {
                Name = fieldName,
                Type = PermissionTypeEnum.ROOT.GetDisplayName(), // Đặt giá trị type tùy thuộc vào yêu cầu của bạn
                Description = PermissionExtension.GetDescriptionByValue(fieldName), // Đặt mô tả nếu có
                IsActive = true, // Đặt giá trị Status tùy thuộc vào yêu cầu của bạn
                ParentId = (int)per.Id, // Lấy giá trị Menu tùy thuộc vào trường field
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            });


            _context.Add(record.Entity);
            _context.SaveChanges();

            return record.Entity;
        }

        public async Task<BaseResponse<object?>> InitPermissionByLogin(string username, string password)
        {
            using (var trans = _context.Database.BeginTransaction())
            {
                try
                {
                    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                    {
                        return new ErrorResponse<object?>("Bad Request");
                    }

                    var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);

                    //check user
                    if (user == null)
                    {
                        return new ErrorResponse<object?>("Tài khoản hoặc mật khẩu không chính xác");
                    }

                    var pass = PasswordHasher.HashPassword(password);

                    //đối chiếu pass
                    if (!PasswordHasher.VerifyPassword(password, user.Password))
                    {
                        return new ErrorResponse<object?>("Tài khoản hoặc mật khẩu không chính xác");
                    }

                    ////check quyền
                    //var roles = await roleRepository.GetListRoleByUserId(user.Id);
                    //if (!roles.Any(x => x.Level == -99))
                    //{
                    //    return new ErrorResponse<object?>("Bạn không có quyền thực hiện");
                    //}

                    //lấy danh sách permission từ class PermissionDefined
                    var listDictPermissions = PermissionExtension.LoadPermissionsTree();
                    ProcessData(listDictPermissions);

                    await trans.CommitAsync();
                    return new BaseResponse<object?>();
                }
                catch (Exception ex)
                {
                    await trans.RollbackAsync();
                    return new ExceptionErrorResponse<object?>(ex, "InitPermission");
                }
            }
        }
        #endregion

        public async Task<Dictionary<string, bool>> GetDictPermissions(int userId)
        {
            try
            {
                var listRoles = await (from us in _context.Users.AsNoTracking()
                                       join ur in _context.UserHasRoles.AsNoTracking() on us.Id equals ur.UserId into urs
                                       from userRoles in urs.DefaultIfEmpty()
                                       join ro in _context.Roles.AsNoTracking() on userRoles.RoleId equals ro.Id into ros
                                       from rol in ros.DefaultIfEmpty()
                                       where us.Id == userId
                                        && userRoles.IsActive == true
                                        && rol.IsActive == true
                                       //orderby rol.Level // Sắp xếp theo level tăng dần
                                       select rol.Id).ToListAsync();

                var allPermissions = await _context.Permissions.ToListAsync();

                var rolePermissions = await _context.RolePermissions
                     .Where(rp => listRoles.Contains(rp.RoleId))
                     .ToListAsync();

                var listPermissions = allPermissions.Select(rp => new
                {
                    PermissionName = rp.Name,
                    HasPermission = rolePermissions.Any(rol => rol.PermissionId == rp.Id)
                }).ToList();

                // Nếu nó có nhiều role và quyền giống nhau thì sẽ ưu tiên quyền là true
                var permissionsDictionary = listPermissions
                                            .GroupBy(p => p.PermissionName)
                                            .Select(group => new
                                            {
                                                PermissionName = group.Key,
                                                HasPermission = group.Any(p => p.HasPermission) // Sử dụng Any để kiểm tra có quyền true hay không
                                            })
                                            .ToDictionary(
                                                s => s.PermissionName,
                                                s => s.HasPermission);

                foreach (var item in allPermissions)
                {
                    if (!permissionsDictionary.ContainsKey(item.Name))
                    {
                        permissionsDictionary[item.Name] = false;
                    }
                }
                return permissionsDictionary;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<BaseResponse<List<Permission>?>> GetAllPermissions()
        {
            try
            {
                var data = _context.Permissions.Where(x => x.IsActive == true)
                    .OrderBy(x => x.Type)
                    .ThenBy(x => x.Id)
                    .ToList();

                return new BaseResponse<List<Permission>?>(data);
            }
            catch (Exception ex)
            {
                return new ExceptionErrorResponse<List<Permission>?>(ex, "GetAllPermission");
            }
        }

        public async Task<BaseResponse<string?>> ScanAndSavePermissions()
        {
            try
            {
                var apiDescriptions = _apiProvider.ApiDescriptionGroups.Items;

                // Lấy tất cả các controller trong assembly
                var controllersWithoutAuthorize = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(type => typeof(ControllerBase).IsAssignableFrom(type) && !type.IsAbstract)
                    .Where(type => !type.GetCustomAttributes(typeof(AuthorizeAttribute), true).Any())
                    .ToList();
                var listController = new List<string>();
                foreach (var controller in controllersWithoutAuthorize)
                {
                    listController.Add(controller.FullName);
                }

                // Lấy danh sách các Permission từ Swagger
                var permissions = apiDescriptions.SelectMany(group => group.Items)
                    .GroupBy(api => $"{api.ActionDescriptor.RouteValues["controller"]}_{api.ActionDescriptor.RouteValues["action"]}")
                    .Select(group => group.First()) // Lấy item đầu tiên trong mỗi nhóm có cùng Name
                    .Select(api => new
                    {
                        ControllerName = api.ActionDescriptor.RouteValues["controller"],
                        ActionName = api.ActionDescriptor.RouteValues["action"],
                        Name = $"{api.ActionDescriptor.RouteValues["controller"]}_{api.ActionDescriptor.RouteValues["action"]}",
                        HttpMethod = api.HttpMethod,
                        Route = api.RelativePath,
                        ModuleName = GetModuleName(api.ActionDescriptor.DisplayName)
                    })
                    .ToList();


                var newPermissions = new List<Permission>();

                foreach (var permission in permissions)
                {
                    // Kiểm tra xem Permission đã tồn tại trong database chưa
                    var exists = _context.Permissions.Any(x => x.Name == permission.Name);
                    if (!exists)
                    {
                        newPermissions.Add(new Permission
                        {
                            Name = permission.Name.ToLower(),
                            ModuleName = permission.ModuleName,
                            Route = permission.Route,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            IsActive = true,
                            Type = "backend"
                        });
                    }
                }

                // Lưu các Permission mới vào database
                if (newPermissions.Any())
                {
                    _context.AddRange(newPermissions);
                    _context.SaveChanges();
                }

                return new BaseResponse<string?>($"Đã thêm {newPermissions.Count} Permission mới.");
            }
            catch (Exception ex)
            {
                return new ExceptionErrorResponse<string?>(ex, "ScanAndSavePermissions");
            }
        }

        private string GetModuleName(string displayName)
        {
            // Example: "NencerApi.Modules.Checkin.Controller.CheckinController"
            var parts = displayName.Split('.');
            int moduleIndex = Array.IndexOf(parts, "Modules") + 1; // Tìm vị trí của "Modules"
            return moduleIndex > 0 && moduleIndex < parts.Length ? parts[moduleIndex] : "Unknown";
        }

    }
}

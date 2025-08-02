using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NencerApi.Helpers;
using NencerApi.Modules.User.Service;
using NencerApi.Modules.User.Model;

namespace NencerApi.Modules.User.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class RoleController
    {
        private readonly RoleService _roleService;

        public RoleController(RoleService roleService)
        {
            this._roleService = roleService;
        }

        [HttpGet("GetAllRole")]
        public async Task<BaseResponse<object>> GetRolesAsync([FromQuery] bool? isActive)
        {
            var item = await _roleService.GetAllRolesToList(isActive);
            return new BaseResponse<object>(item);
        }

        [HttpGet("GetRoleById/{id}")]
        public async Task<BaseResponse<Role>> GetRoleById(int? id)
        {
            return await _roleService.GetRoleById(id);
        }

        [HttpGet("GetUserInfoAndRolesByUserId/{userId}")]
        public async Task<BaseResponse<UserInfoAndRoles>> GetUserInfoAndRolesByUserId(int userId)
        {
            return await _roleService.GetUserInfoAndRolesByUserId(userId);
        }

        [HttpGet("GetListRolePermissionByRoleId/{roleId}")]
        public async Task<BaseResponse<List<RolePermission>?>> GetListRolePermissionByRoleId(int roleId)
        {
            return await _roleService.GetListRolePermissionByRoleId(roleId);
        }

        [HttpPost("CreateRoleWithPermissions")]
        public async Task<BaseResponse<object?>> CreateRoleWithPermissions(RoleWithPermissionsDto req)
        {
            return await _roleService.CreateRoleWithPermissions(req);
        }

        [HttpPost("UpdateRoleWithPermissions")]
        public async Task<BaseResponse<object?>> UpdateRoleWithPermissions(RoleWithPermissionsDto req)
        {
            return await _roleService.UpdateRoleWithPermissions(req);
        }

        [HttpPost("UpdateRolesForUser")]
        public async Task<BaseResponse<object?>> UpdateRolesForUser(UserAndRolesReq req)
        {
            return await _roleService.UpdateRolesForUser(req);
        }

        //// add role permission
        //[HttpGet("GetAll")]
        //public async Task<BaseResponse<List<Role>>> GetAllRoles(bool? isActive)
        //{
        //    return await _roleService.GetAllRoles(isActive);
        //}
    }
}

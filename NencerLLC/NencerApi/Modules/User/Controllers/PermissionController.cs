using DevExpress.XtraSpreadsheet.Internal;
using Microsoft.AspNetCore.Mvc;
using NencerApi.Helpers;
using NencerApi.Modules.User.Service;
using NencerApi.Modules.User.Model;

namespace NencerApi.Modules.User.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionController
    {
        private PermissionService _permissionService;

        public PermissionController(PermissionService permissionService)
        {
            this._permissionService = permissionService;
        }

        //[HttpPost("InitPermissionByLogin")]
        //public async Task<BaseResponse<object?>> InitPermissionByLogin(string userName, string password)
        //{
        //    return await _permissionService.InitPermissionByLogin(userName, password);
        //}

        [HttpPost("GetAllPermissions")]
        public async Task<BaseResponse<List<Permission>?>> GetAllPermissions()
        {
            return await _permissionService.GetAllPermissions();
        }

        [HttpPost("ScanAndSavePermissions")]
        public async Task<BaseResponse<string?>> ScanAndSavePermissions([FromQuery] string pass)
        {
            if (pass != "nencer@150")
            {
                return new ErrorResponse<string?>("wrong_pass");
            }
            return await _permissionService.ScanAndSavePermissions();
        }
    }
}

using Microsoft.AspNetCore.Mvc.Filters;
using NencerApi.Helpers;
using NencerApi.Modules.User.Service;
using Newtonsoft.Json;

namespace NencerApi.ActionFilter
{
    public class LogActionFilter : IActionFilter
    {
        private readonly CheckPermissionService _checkPermissionService;

        public LogActionFilter(CheckPermissionService checkPermissionService)
        {
            _checkPermissionService = checkPermissionService;
        }

        public async void OnActionExecuting(ActionExecutingContext context)
        {
            // Mặc định giá trị cho người dùng không được xác thực
            var reqUser = "UnAuthorized";

            // Lấy thông tin người dùng từ Claims nếu có
            var userNameClaim = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Username");
            if (userNameClaim != null)
            {
                reqUser = userNameClaim.Value;
            }

            // Lấy tên controller và action
            var controllerName = context.RouteData.Values["controller"]?.ToString()?.ToLower();
            var actionName = context.RouteData.Values["action"]?.ToString()?.ToLower();

            // Serialize thông tin request
            var requestInfo = context.ActionArguments.Count > 0 ? JsonConvert.SerializeObject(context.ActionArguments) : "No arguments";


            ////check permission
            //// Kiểm tra xem action hoặc controller có [Authorize] hay không , nếu [Authorize] mới check permission
            //var isAuthorized = context.ActionDescriptor.EndpointMetadata
            //    .Any(metadata => metadata is Microsoft.AspNetCore.Authorization.AuthorizeAttribute);
            //if (isAuthorized)
            //{
            //    // nếu không phải là đầu controller tạo quyền thì sẽ check quyền
            //    // đầu api này có pass cố định riêng
            //    if (controllerName != "Permission".ToLower() || actionName != "ScanAndSavePermissions".ToLower())
            //    {
            //        // Lấy danh sách roles từ token
            //        var roles = context.HttpContext.User.Claims
            //            .Where(c => c.Type == "Role")
            //            .Select(c => c.Value)
            //            .ToList();

            //        // Tạo permission cần kiểm tra
            //        var requiredPermission = $"{controllerName}_{actionName}";

            //        // Kiểm tra quyền requiredPermission có nằm trong các role của khách không


            //        if (!(await _checkPermissionService.HasPermission(requiredPermission, roles)))
            //        {
            //            // Ghi log
            //            Helpers.LogHelper.Info($"NoPermissionRequest ({requiredPermission}) : User: {reqUser}, Controller: {controllerName}, Action: {actionName}, Arguments: {requestInfo}");

            //            context.Result = new Microsoft.AspNetCore.Mvc.OkObjectResult(new NoPermissionErrorResponse<object>());
            //            return;
            //        }
            //    }
            //}

            // Ghi log với tất cả thông tin thu thập được
            Helpers.LogHelper.Info($"User: {reqUser}, Controller: {controllerName}, Action: {actionName}, Arguments: {requestInfo}");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Log sau khi action thực thi xong
            Helpers.LogHelper.Info($"Action {context.ActionDescriptor.DisplayName} đã thực thi xong.");
        }
    }
}

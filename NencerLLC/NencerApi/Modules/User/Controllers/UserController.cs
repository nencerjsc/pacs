using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NencerApi.Helpers;
using NencerApi.Modules.User.Service;
using NencerApi.Modules.User.Model;
using NencerCore;

namespace NencerApi.Modules.User.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly RoleService _roleService;

        public UserController(AppDbContext context, IMapper mapper, RoleService roleService)
        {
            _context = context;
            _mapper = mapper;
            _roleService = roleService;
        }

        // GET: api/User
        [HttpGet("GetAll")]
        public async Task<ActionResult<BaseResponseList<List<UserModel>>>> GetAll(
            [FromQuery] DateTime? startdate,
            [FromQuery] DateTime? enddate,
            [FromQuery] string? keyword,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10
            )
        {
            var query = _context.Users.AsNoTracking();

            //var filter = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.Trim().ToLower();
                query = query.Where(x => (x.Name != null && x.Name.ToLower().Contains(keyword)) || (x.Username != null && x.Username.ToLower().Contains(keyword)));
            }
            var skip = (page - 1) * limit;
            int totalRecord = await query.CountAsync();
            int totalPage = (int)Math.Ceiling(totalRecord / (double)limit);
            var users = query.Skip(skip).Take(limit).ToList();

            users.ForEach(x => x.Password = "");
            users.ForEach(x => x.Signature = "");

            var response = new BaseResponseList<List<UserModel>>("200", "success", users, page, limit, totalPage);
            return Ok(response);
        }

        [HttpGet("GetAllUserDto")]
        public async Task<ActionResult<BaseResponseList<List<UserDto>>>> GetAllUserDto(
            [FromQuery] string? keyword,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10
            )
        {
            var query = from user in _context.Users
                        where (string.IsNullOrEmpty(keyword)
                            || (user.Name != null && user.Name.ToLower().Contains(keyword.Trim().ToLower()))
                            || (user.Username != null && user.Username.ToLower().Contains(keyword.Trim().ToLower()))
                            || (user.Phone != null && user.Phone.ToLower().Contains(keyword.Trim().ToLower()))
                            )
                        select new UserDto
                        {
                            Id = user.Id,
                            Name = user.Name,
                            UserName = user.Username
                        };

            var skip = (page - 1) * limit;
            int totalRecord = await query.CountAsync();
            int totalPage = (int)Math.Ceiling(totalRecord / (double)limit);
            var users = query.Skip(skip).Take(limit).ToList();

            var response = new BaseResponseList<List<UserDto>>("200", "success", users, page, limit, totalPage);
            return Ok(response);
        }

        // GET: api/User/5
        [HttpGet("GetById/{id}")]
        public async Task<BaseResponse<UserReqDto>> GetById(int id)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(x => x.Id == id);
                if (user == null)
                {
                    return new ErrorResponse<UserReqDto>("user_not_found");
                }
                var data = _mapper.Map<UserReqDto>(user);
                data.Password = "";

                //lấy list roleIds
                data.RoleIds = (await _roleService.GetListRoleByUserId(id)).Select(x => x.Id).ToList();
                return new BaseResponse<UserReqDto>(data);
            }
            catch (Exception ex)
            {
                return new ExceptionErrorResponse<UserReqDto>(ex, "GetUserById");
            }
        }

        // POST: api/User
        [HttpPost("Create")]
        public async Task<BaseResponse<UserReqDto>> Create(UserReqDto userReq)
        {
            using (var trans = _context.Database.BeginTransaction())
            {
                try
                {
                    var username = userReq.Username;
                    string pattern = @"^[a-zA-Z0-9_]{3,20}$";
                    bool isValid = Regex.IsMatch(username, pattern);

                    if (!isValid)
                    {
                        trans.Rollback();
                        return new ErrorResponse<UserReqDto>("invalid_username");
                    }

                    if (_context.Users.Any(u => u.Username == userReq.Username))
                    {
                        trans.Rollback();
                        return new ErrorResponse<UserReqDto>("user_name_existed");
                    }

                    var userModel = _mapper.Map<UserModel>(userReq);
                    //mã hóa pass
                    userModel.Password = BCrypt.Net.BCrypt.HashPassword(userModel.Password, 10);
                    _context.User.Add(userModel);
                    await _context.SaveChangesAsync(); //lưu để lấy userid

                    //add role
                    var roleIds = userReq.RoleIds;
                    if (roleIds != null && roleIds.Any())
                    {
                        //check tồn tại
                        bool allExist = roleIds.All(roleId => _context.Roles.Any(r => r.Id == roleId && r.IsActive == true));
                        if (!allExist)
                        {
                            trans.Rollback();
                            return new ErrorResponse<UserReqDto>("role_not_existed_or_deleted");
                        }

                        //tạo user - role
                        var createRoleReq = new UserAndRolesReq();
                        createRoleReq.ListRoleId = roleIds;
                        createRoleReq.UserId = userModel.Id;
                        var createRole_User = await _roleService.UpdateRolesForUser(createRoleReq);
                        if (createRole_User.Status != "200")
                        {
                            trans.Rollback();
                            return new ErrorResponse<UserReqDto>(createRole_User.Message);
                        }
                    }
                    await _context.SaveChangesAsync();
                    trans.Commit();
                    return new BaseResponse<UserReqDto>("200", "success", userReq);
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return new ExceptionErrorResponse<UserReqDto>(ex, "CreateUser");
                }
            }
        }

        // PUT: api/User/5
        [HttpPost("Update")]
        public async Task<BaseResponse<UserModel>> Update(UserReqDto userModel)
        {
            using (var trans = _context.Database.BeginTransaction())
            {
                try
                {
                    //kiểm tra tòn tại
                    var user = await _context.User.FindAsync(userModel.Id);
                    if (user == null)
                    {
                        trans.Rollback();
                        return new ErrorResponse<UserModel>("user_not_found");
                    }

                    //update các thông tin được phép cập nhật
                    user.Name = userModel.Name;
                    user.Email = userModel.Email;
                    user.Signature = userModel.Signature;
                    user.Phone = userModel.Phone;
                    user.IsActive = userModel.IsActive;

                    _context.Update(user);

                    //update role
                    //check tồn tại
                    var roleIds = userModel.RoleIds ?? new List<int>();
                    bool allExist = roleIds.All(roleId => _context.Roles.Any(r => r.Id == roleId && r.IsActive == true));
                    if (!allExist)
                    {
                        trans.Rollback();
                        return new ErrorResponse<UserModel>("role_not_existed_or_deleted");
                    }
                    var createRoleReq = new UserAndRolesReq();
                    createRoleReq.ListRoleId = userModel.RoleIds;
                    createRoleReq.UserId = userModel.Id;
                    var createRole_User = await _roleService.UpdateRolesForUser(createRoleReq);
                    if (createRole_User.Status != "200")
                    {
                        trans.Rollback();
                        return new ErrorResponse<UserModel>(createRole_User.Message);
                    }
                    _context.SaveChanges();
                    trans.Commit();
                    return new BaseResponse<UserModel>(user);
                }
                catch (Exception ex)
                {
                    return new ExceptionErrorResponse<UserModel>(ex, "UpdateUser");
                }
            }
        }

        // DELETE: api/User/5
        [HttpDelete("Delete/{id}")]
        public async Task<BaseResponse<object>> Delete(int id)
        {
            using (var trans = _context.Database.BeginTransaction())
            {
                try
                {
                    //check user
                    var user = await _context.User.FindAsync(id);
                    if (user == null)
                    {
                        trans.Rollback();
                        return new ErrorResponse<object>("user_not_found");
                    }

                    //delete role của user
                    var roles = await _context.UserHasRoles.Where(x => x.UserId == user.Id).ToListAsync();
                    if (roles != null)
                    {
                        _context.RemoveRange(roles);
                    }

                    _context.Remove(user);

                    _context.SaveChanges();
                    trans.Commit();
                    return new BaseResponse<object>();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return new ExceptionErrorResponse<object>(ex, "DeleteUser");
                }
            }
        }

        private bool UserModelExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}

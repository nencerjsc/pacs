using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NencerApi.Helpers;
using NencerApi.Modules.SystemNc.Model;
using NencerApi.Modules.User.Model;
using NencerCore;
using System.Net.WebSockets;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NencerApi.Modules.User.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        public readonly AppDbContext _appdbContext;

        public UserRoleController(AppDbContext appdbContext)
        {
            _appdbContext = appdbContext;
        }


        // GET: api/<UserRoleController>
        [HttpGet("GetAll")]
        public async Task<ActionResult<BaseResponseList<List<Role>>>> GetAll(
               [FromQuery] DateTime? startDate,
               [FromQuery] DateTime? endDate,
               [FromQuery] string? name,
               [FromQuery] int pageNumber,
               [FromQuery] int pageSize 
            )
        {
            if (_appdbContext.Roles == null)
            {
                return NotFound(new BaseResponse<Role>("404", "not_found", null));
            }

            var query = _appdbContext.Roles.AsQueryable();
            var filter = new Dictionary<string , object>();
            if (!string.IsNullOrEmpty(name)) {
                filter.Add(nameof(Role.Name) , name);
            }
            query = query.ApplyFilter(filter);
            if (startDate != null && endDate != null)
            {
                query = query.ApplyDateRangeFilter(s => (DateTime)(s.CreatedAt??DateTime.Now), startDate, endDate);

            }

            int totalRecord = await query.CountAsync();
            int totalPage = (int)Math.Ceiling(totalRecord / (double)pageSize);
            var userRole = await query.ApplyPagination(pageNumber, pageSize).ToListAsync();

            var repone = new BaseResponseList<List<Role>>("200", "Suscess", userRole.ToList(), pageNumber, pageSize, totalPage);
            return Ok(repone); 
        }

        // GET api/<UserRoleController>/5
        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<Role>> GetById(int id)
        {
            if (_appdbContext.Roles == null)
            {
                return NotFound(new BaseResponse<Role>("404", "not_found", null));
            }

            var userID = await _appdbContext.Roles.FindAsync(id);
            if(userID == null)
            {
                return NotFound(new BaseResponse<Role>("404", "not_found", null));
            }
            return Ok(new BaseResponse<Role>("200", "Success", userID));
        }

        // POST api/<UserRoleController>
        [HttpPost("Create")]
        public async Task<ActionResult<Role>> Create(Role userRoleModel)
        {
            if (_appdbContext.Roles == null)
            {
                return NotFound(new BaseResponse<Role>("404", "not_found", null));
            }
            _appdbContext.Roles.Add(userRoleModel);
            await _appdbContext.SaveChangesAsync();

            return Ok(new BaseResponse<Role>("200", "Success", userRoleModel));
        }

        // PUT api/<UserRoleController>/5
        [HttpPut("Update/{id}")]
        public async Task<ActionResult<Role>> Update(int id, Role userRoleModel)
        {
            if (_appdbContext.Roles == null)
            {
                return NotFound(new BaseResponse<Role>("404", "not_found", null));
            }

            _appdbContext.Entry(userRoleModel).State = EntityState.Modified;
            try
            {
                await _appdbContext.SaveChangesAsync();
            }
            catch (DbUpdateException) {
                if (!UserRoleExits(id))
                {
                    return NotFound(new BaseResponse<Role>("404", "not_found", null));
                }
            }

            return Ok(new BaseResponse<Role>("200", "Success", userRoleModel));
        }

        // DELETE api/<UserRoleController>/5
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<Role>> Delete(int id)
        {
            if (_appdbContext.Roles == null)
            {
                return NotFound(new BaseResponse<Role>("404", "not_found", null));
            }
            var userID = await _appdbContext.Roles.FindAsync(id);
            if (userID == null)
            {
                return NotFound(new BaseResponse<Role>("404", "not_found", null));
            }
            _appdbContext.Roles.Remove(userID);
            await _appdbContext.SaveChangesAsync();

            return Ok(new BaseResponse<Role>("200", "Success", userID));
        }

        private bool UserRoleExits(int id)
        {

            return (_appdbContext.Roles?.Any(u => u.Id == id)).GetValueOrDefault();
        }
    }
}

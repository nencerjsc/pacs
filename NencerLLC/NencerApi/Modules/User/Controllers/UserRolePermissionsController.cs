using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NencerApi.Helpers;
using NencerApi.Modules.User.Model;
using NencerCore;

namespace NencerApi.Modules.User.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRolePermissionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserRolePermissionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/UserRolePermissions
        [HttpGet("GetAll")]
        public async Task<ActionResult<BaseResponseList<List<RolePermission>>>> GetAll(
               [FromQuery] DateTime? startDate,
               [FromQuery] DateTime? endDate,
               [FromQuery] int pageNumber=1,
               [FromQuery] int pageSize =10
            )
        {
            if (_context.RolePermissions == null)
            {
                return NotFound(new BaseResponse<RolePermission>("404", "not_found", null));
            }

            var query = _context.RolePermissions.AsQueryable();
           
            if (startDate != null && endDate != null)
            {
                query = query.ApplyDateRangeFilter(s => (DateTime)(s.CreatedAt ?? DateTime.Now), startDate, endDate);
            }

            int totalRecord = await query.CountAsync();
            int totalPage = (int)Math.Ceiling(totalRecord / (double)pageSize);
            var userRolePermissions = await query.ApplyPagination(pageNumber, pageSize).ToListAsync();

            var repone = new BaseResponseList<List<RolePermission>>("200", "Suscess", userRolePermissions, pageNumber, pageSize, totalPage);
            return Ok(repone);
        }

        // GET: api/UserRolePermissions/5
        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<RolePermission>> GetById(int id)
        {
            if (_context.RolePermissions == null)
            {
                return NotFound(new BaseResponse<RolePermission>("404", "not_found", null));
            }
            var userRolePermissions = await _context.RolePermissions.FindAsync(id);

            if (userRolePermissions == null)
            {
                return NotFound(new BaseResponse<RolePermission>("404", "not_found", null));
            }

            return Ok(new BaseResponse<RolePermission>("200", "success", userRolePermissions));
        }

        // PUT: api/UserRolePermissions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, RolePermission userRolePermissions)
        {
            if (_context.RolePermissions == null)
            {
                return NotFound(new BaseResponse<RolePermission>("404", "not_found", null));
            }
            if (id != userRolePermissions.Id)
            {
                return BadRequest(new BaseResponse<RolePermission>("400", "invalid_input_data", null));
            }

            _context.Entry(userRolePermissions).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserRolePermissionsExists(id))
                {
                    return NotFound(new BaseResponse<RolePermission>("404", "not_found", null));
                }
                else
                {
                    throw;
                }
            }

            return Ok(new BaseResponse<RolePermission>("200", "success", userRolePermissions));
        }

        // POST: api/UserRolePermissions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Create")]
        public async Task<ActionResult<RolePermission>> Create(RolePermission userRolePermissions)
        {
            if (_context.RolePermissions == null)
            {
                return NotFound(new BaseResponse<RolePermission>("404", "not_found", null));
            }
            _context.RolePermissions.Add(userRolePermissions);
            await _context.SaveChangesAsync();

            return Ok(new BaseResponse<RolePermission>("200", "success", userRolePermissions));
        }

        // DELETE: api/UserRolePermissions/5
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> HttpDelete(int id)
        {
            if (_context.RolePermissions == null)
            {
                return NotFound(new BaseResponse<RolePermission>("404", "not_found", null));
            }
            var userRolePermissions = await _context.RolePermissions.FindAsync(id);
            if (userRolePermissions == null)
            {
                return NotFound(new BaseResponse<RolePermission>("404", "not_found", null));
            }

            _context.RolePermissions.Remove(userRolePermissions);
            await _context.SaveChangesAsync();

            return Ok(new BaseResponse<RolePermission>("200", "success", userRolePermissions));
        }

        private bool UserRolePermissionsExists(int id)
        {
            return (_context.RolePermissions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

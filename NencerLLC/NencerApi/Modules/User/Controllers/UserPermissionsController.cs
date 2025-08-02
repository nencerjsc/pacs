using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class UserPermissionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserPermissionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/UserPermissions
        [HttpGet("GetAll")]
        public async Task<ActionResult<BaseResponseList<List<Permission>>>> GetAll(
                [FromQuery] string? name,
                [FromQuery] DateTime? startDate,
                [FromQuery] DateTime? endDate,
                [FromQuery] int pageNumber =1,
                [FromQuery] int pageSize= 10
             )
        {
            if(_context.Permissions == null)
            {
                return NotFound(new BaseResponse<Permission>("404", "not_found", null));
            }
            var query = _context.Permissions.AsQueryable();
            var filter = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(name)) { 
                filter.Add(nameof(Permission.Name), name);
            }
            query = query.ApplyFilter(filter);
            if (startDate != null && endDate != null) { 
                query =query.ApplyDateRangeFilter(u=> (DateTime)(u.CreatedAt??DateTime.Now), startDate, endDate);
            }

            int totaRecord = await query.CountAsync();
            int totalPage = (int)Math.Ceiling(totaRecord / (double)pageSize);
            var userP = await query.ApplyPagination(pageNumber, pageSize).ToListAsync();

            var repone = new BaseResponseList<List<Permission>>("200", "Succes", userP, pageNumber, pageSize, totaRecord);
            return Ok(repone);
        }

        // GET: api/UserPermissions/5
        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<Permission>> GetById(int id)
        {
            if (_context.Permissions == null)
            {
                return NotFound(new BaseResponse<Permission>("404", "not_found", null));
            }
            var userPermissions = await _context.Permissions.FindAsync(id);

            if (userPermissions == null)
            {
                return NotFound(new BaseResponse<Permission>("404", "not_found", null));
            }

            return Ok(new BaseResponse<Permission>("200", "success", userPermissions));
        }

        // PUT: api/UserPermissions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, Permission userPermissions)
        {
            if (_context.Permissions == null)
            {
                return NotFound(new BaseResponse<Permission>("404", "not_found", null));
            }
            if (id != userPermissions.Id)
            {
                return BadRequest(new BaseResponse<UserModel>("400", "invalid_input_data", null));
            }

            _context.Entry(userPermissions).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserPermissionsExists(id))
                {
                    return NotFound(new BaseResponse<Permission>("404", "not_found", null));
                }
                else
                {
                    throw;
                }
            }

            return Ok(new BaseResponse<Permission>("200", "success", userPermissions));
        }

        // POST: api/UserPermissions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Create")]
        public async Task<ActionResult<Permission>> Create(Permission userPermissions)
        {
            if (_context.Permissions == null)
            {
                return NotFound(new BaseResponse<Permission>("404", "not_found", null));
            }
            _context.Permissions.Add(userPermissions);
            await _context.SaveChangesAsync();

            return Ok(new BaseResponse<Permission>("200", "success", userPermissions));
        }

        // DELETE: api/UserPermissions/5
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.Permissions == null)
            {
                return NotFound(new BaseResponse<Permission>("404", "not_found", null));
            }
            var userPermissions = await _context.Permissions.FindAsync(id);
            if (userPermissions == null)
            {
                return NotFound(new BaseResponse<Permission>("404", "not_found", null));
            }

            _context.Permissions.Remove(userPermissions);
            await _context.SaveChangesAsync();

            return Ok(new BaseResponse<Permission>("200", "success", userPermissions));
        }

        private bool UserPermissionsExists(int id)
        {
            return (_context.Permissions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

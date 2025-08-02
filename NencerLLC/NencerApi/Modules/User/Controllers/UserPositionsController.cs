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
    public class UserPositionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserPositionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/UserPositions
        [HttpGet("GetAll")]
        public async Task<ActionResult<BaseResponseList<UserPositionsModel>>> GetAll(
            [FromQuery] DateTime? startdate,
            [FromQuery] DateTime? enddate,
            [FromQuery] string? namesearch,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10            
            )
        {
            if (_context.UserPositionsModel == null) 
            {
                return NotFound(new BaseResponse<UserPositionsModel>("404", "not_found", null));
            }
            var query = _context.UserPositionsModel.AsQueryable();
            var filter = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(namesearch))
            {
                filter.Add(nameof(UserPositionsModel.NameSearch), namesearch);
            }

            query = query.ApplyFilter(filter);
            if (startdate != null && enddate != null)
            {
                query = query.ApplyDateRangeFilter(a => a.CreatedAt ?? DateTime.Now, startdate, enddate);
            }
            int totalRecord = await query.CountAsync();
            int totalPage = (int)Math.Ceiling(totalRecord / (double)limit);

            var position = await query.ApplyPagination(page, limit).ToListAsync();
            var repone = new BaseResponseList<List<UserPositionsModel>>("200", "Success", position, page, limit, totalPage);
            return Ok(repone);
        }

        // GET: api/UserPositions/5
        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<UserPositionsModel>> GetById(int id)
        {
            if (_context.UserPositionsModel == null)
            {
                return NotFound(new BaseResponse<UserPositionsModel>("404", "not_found", null));
            }
            var userPositionsModel = await _context.UserPositionsModel.FindAsync(id);

            if (userPositionsModel == null)
            {
                return NotFound(new BaseResponse<UserPositionsModel>("404", "not_found", null));
            }

            return Ok(new BaseResponse<UserPositionsModel>("200", "success", userPositionsModel));
        }

        // PUT: api/UserPositions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, UserPositionsModel userPositionsModel)
        {
            if (id != userPositionsModel.Id)
            {
                return BadRequest(new BaseResponse<UserPositionsModel>("400","invalid_input_data", null));
            }
            userPositionsModel.UpdatedAt = DateTime.Now;
            _context.Entry(userPositionsModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserPositionsModelExists(id))
                {
                    return NotFound(new BaseResponse<UserPositionsModel>("404", "not_found", null));
                }
                else
                {
                    throw;
                }
            }

            return Ok(new BaseResponse<UserPositionsModel>("200", "success", userPositionsModel));
        }

        // POST: api/UserPositions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Create")]
        public async Task<ActionResult<UserPositionsModel>> Create(UserPositionsModel userPositionsModel)
        {
            if (_context.UserPositionsModel == null)
            {
                return NotFound(new BaseResponse<UserPositionsModel>("404", "not_found", null));
            }
            userPositionsModel.CreatedAt = DateTime.Now;
            userPositionsModel.UpdatedAt= DateTime.Now;
            _context.UserPositionsModel.Add(userPositionsModel);
            await _context.SaveChangesAsync();

            return Ok(new BaseResponse<UserPositionsModel>("200", "success", userPositionsModel));
        }

        // DELETE: api/UserPositions/5
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.UserPositionsModel == null)
            {
                return NotFound(new BaseResponse<UserPositionsModel>("404", "not_found", null));
            }
            var userPositionsModel = await _context.UserPositionsModel.FindAsync(id);
            if (userPositionsModel == null)
            {
                return NotFound(new BaseResponse<UserPositionsModel>("404", "not_found", null));
            }

            _context.UserPositionsModel.Remove(userPositionsModel);
            await _context.SaveChangesAsync();

            return Ok(new BaseResponse<UserPositionsModel>("200", "success", userPositionsModel));
        }

        private bool UserPositionsModelExists(int id)
        {
            return _context.UserPositionsModel.Any(e => e.Id == id);
        }
    }
}

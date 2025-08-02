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
    public class UserJobReviewsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserJobReviewsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/UserJobReviews
        [HttpGet("Getall")]
        public async Task<ActionResult<BaseResponseList<UserJobReviewsModel>>> Getall(
            [FromQuery] DateTime? startdate,
            [FromQuery] DateTime? enddate,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10
            )
        {
            if(_context.UserJobReviewsModel == null)
            {
                return NotFound(new BaseResponse<UserJobReviewsModel>("404", "not_found", null));
            }
            var query = _context.UserJobReviewsModel.AsQueryable();
            if (startdate != null || enddate != null)
            {
                query = query.ApplyDateRangeFilter(a => a.CreatedAt ?? DateTime.Now, startdate, enddate);
            }
            int totalRecord = await query.CountAsync();
            int totalPage = (int)Math.Ceiling(totalRecord / (double)limit);
            var jobrview = await query.ApplyPagination(page, limit).ToListAsync();
            var response = new BaseResponseList<List<UserJobReviewsModel>>("200", "success", jobrview, page, limit, totalPage);
            return Ok(response);
        }

        // GET: api/UserJobReviews/5
        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<UserJobReviewsModel>> GetById(int id)
        {
            if (_context.UserJobReviewsModel == null)
            {
                return NotFound(new BaseResponse<UserJobReviewsModel>("404", "not_found", null));
            }
            var userJobReviewsModel = await _context.UserJobReviewsModel.FindAsync(id);

            if (userJobReviewsModel == null)
            {
                return NotFound(new BaseResponse<UserJobReviewsModel>("404", "not_found", null));
            }

            return Ok(new BaseResponse<UserJobReviewsModel>("200", "success", userJobReviewsModel));
        }

        // PUT: api/UserJobReviews/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, UserJobReviewsModel userJobReviewsModel)
        {
            if (id != userJobReviewsModel.Id)
            {
                return BadRequest(new BaseResponse<UserJobReviewsModel>("400", "invalid_input_data", null));
            }
            userJobReviewsModel.UpdatedAt = DateTime.Now;
            _context.Entry(userJobReviewsModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserJobReviewsModelExists(id))
                {
                    return NotFound(new BaseResponse<UserJobReviewsModel>("404", "not_found", null));
                }
                else
                {
                    throw;
                }
            }

            return Ok(new BaseResponse<UserJobReviewsModel>("200", "success", userJobReviewsModel));
        }

        // POST: api/UserJobReviews
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Create")]
        public async Task<ActionResult<UserJobReviewsModel>> Create(UserJobReviewsModel userJobReviewsModel)
        {
            if (_context.UserJobReviewsModel == null)
            {
                return NotFound(new BaseResponse<UserJobReviewsModel>("404", "not_found", null));
            }
            userJobReviewsModel.CreatedAt = DateTime.Now;
            userJobReviewsModel.UpdatedAt= DateTime.Now;           
            _context.UserJobReviewsModel.Add(userJobReviewsModel);
            await _context.SaveChangesAsync();

            return Ok(new BaseResponse<UserJobReviewsModel>("200", "success", userJobReviewsModel));
        }

        // DELETE: api/UserJobReviews/5
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.UserJobReviewsModel == null)
            {
                return NotFound(new BaseResponse<UserJobReviewsModel>("404", "not_found", null));
            }
            var userJobReviewsModel = await _context.UserJobReviewsModel.FindAsync(id);
            if (userJobReviewsModel == null)
            {
                return NotFound(new BaseResponse<UserJobReviewsModel>("404", "not_found", null));
            }

            _context.UserJobReviewsModel.Remove(userJobReviewsModel);
            await _context.SaveChangesAsync();

            return Ok(new BaseResponse<UserJobReviewsModel>("200", "success", userJobReviewsModel));
        }

        private bool UserJobReviewsModelExists(int id)
        {
            return _context.UserJobReviewsModel.Any(e => e.Id == id);
        }
    }
}

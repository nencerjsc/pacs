using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NencerApi.Helpers;
using NencerApi.Modules.SystemNc.Model;
using NencerCore;

namespace NencerApi.Modules.SystemNc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DigiSignLogController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DigiSignLogController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<BaseResponseList<List<DigiSignLog>>>> GetAll(
            [FromQuery] string? provider,
            [FromQuery] int? userId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _context.DigiSignLogs.AsQueryable();

            if (!string.IsNullOrEmpty(provider))
                query = query.Where(x => x.Provider.Contains(provider));

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId);

            int total = await query.CountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)pageSize);

            var data = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new BaseResponseList<List<DigiSignLog>>("200", "Success", data, pageNumber, pageSize, totalPage));
        }

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<BaseResponse<DigiSignLog>>> GetById(long id)
        {
            var item = await _context.DigiSignLogs.FindAsync(id);
            if (item == null)
                return NotFound(new BaseResponse<DigiSignLog>("404", "Not Found", null));

            return Ok(new BaseResponse<DigiSignLog>("200", "Success", item));
        }

        [HttpPost("Create")]
        public async Task<ActionResult<BaseResponse<DigiSignLog>>> Create(DigiSignLog model)
        {
            model.CreatedAt = DateTime.Now;
            _context.DigiSignLogs.Add(model);
            await _context.SaveChangesAsync();
            return Ok(new BaseResponse<DigiSignLog>("200", "Created", model));
        }

        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<BaseResponse<DigiSignLog>>> Delete(long id)
        {
            var item = await _context.DigiSignLogs.FindAsync(id);
            if (item == null)
                return NotFound(new BaseResponse<DigiSignLog>("404", "Not Found", null));

            _context.DigiSignLogs.Remove(item);
            await _context.SaveChangesAsync();
            return Ok(new BaseResponse<DigiSignLog>("200", "Deleted", item));
        }
    }
}

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
    public class DigiSignProviderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DigiSignProviderController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<BaseResponseList<List<DigiSignProvider>>>> GetAll(
            [FromQuery] string? name,
            [FromQuery] string? code,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _context.DigiSignProviders.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(x => x.Name.Contains(name));

            if (!string.IsNullOrEmpty(code))
                query = query.Where(x => x.Code.Contains(code));

            int total = await query.CountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)pageSize);

            var data = await query
                .OrderBy(x => x.Sort)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new BaseResponseList<List<DigiSignProvider>>("200", "Success", data, pageNumber, pageSize, totalPage));
        }

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<BaseResponse<DigiSignProvider>>> GetById(int id)
        {
            var item = await _context.DigiSignProviders.FindAsync(id);
            if (item == null)
                return NotFound(new BaseResponse<DigiSignProvider>("404", "Not Found", null));

            return Ok(new BaseResponse<DigiSignProvider>("200", "Success", item));
        }

        [HttpPost("Create")]
        public async Task<ActionResult<BaseResponse<DigiSignProvider>>> Create(DigiSignProvider model)
        {
            _context.DigiSignProviders.Add(model);
            await _context.SaveChangesAsync();
            return Ok(new BaseResponse<DigiSignProvider>("200", "Created", model));
        }

        [HttpPut("Update/{id}")]
        public async Task<ActionResult<BaseResponse<DigiSignProvider>>> Update(int id, DigiSignProvider model)
        {
            if (id != model.Id)
                return BadRequest(new BaseResponse<DigiSignProvider>("400", "Invalid ID", null));

            _context.Entry(model).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.DigiSignProviders.Any(x => x.Id == id))
                    return NotFound(new BaseResponse<DigiSignProvider>("404", "Not Found", null));
                throw;
            }

            return Ok(new BaseResponse<DigiSignProvider>("200", "Updated", model));
        }

        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<BaseResponse<DigiSignProvider>>> Delete(int id)
        {
            var item = await _context.DigiSignProviders.FindAsync(id);
            if (item == null)
                return NotFound(new BaseResponse<DigiSignProvider>("404", "Not Found", null));

            _context.DigiSignProviders.Remove(item);
            await _context.SaveChangesAsync();
            return Ok(new BaseResponse<DigiSignProvider>("200", "Deleted", item));
        }
    }
}

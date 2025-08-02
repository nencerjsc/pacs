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
    public class SendmessController : ControllerBase
    {

        private readonly AppDbContext _context;

        public SendmessController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Sendmess
        [HttpGet]
        public async Task<ActionResult<BaseResponseList<List<SendmessModel>>>> GetSendmesses(
                    [FromQuery] string? name,
                    [FromQuery] int? status,
                    [FromQuery] int pageNumber = 1,
                    [FromQuery] int pageSize = 10)
        {
            if (_context.Sendmesses == null)
            {
                return NotFound(new BaseResponse<SendmessModel>("404", "not_found", null));
            }

            var query = _context.Sendmesses.AsQueryable();

            var filters = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(name))
            {
                filters.Add(nameof(SendmessModel.Name), name);
            }

            if (status.HasValue)
            {
                filters.Add(nameof(SendmessModel.Status), status);
            }

            query = query.ApplyFilter(filters);
  
            int totalRecord = await query.CountAsync();
            int totalPage = (int)Math.Ceiling(totalRecord / (double)pageSize);
            var sendmesses = await query.ApplyPagination(pageNumber, pageSize).ToListAsync();

            var response = new BaseResponseList<List<SendmessModel>>("200", "success", sendmesses, pageNumber, pageSize, totalPage);
            return Ok(response);
        }

        // GET: api/Sendmess/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SendmessModel>> GetSendmess(int id)
        {
            if (_context.Sendmesses == null)
            {
                return NotFound(new BaseResponse<SendmessModel>("404", "not_found", null));
            }
            var sendmess = await _context.Sendmesses.FindAsync(id);

            if (sendmess == null)
            {
                return NotFound(new BaseResponse<List<SendmessModel>>("404", "not_found", null));
            }

            return Ok(new BaseResponse<SendmessModel>("200", "success", sendmess));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSendmess(int id, SendmessModel sendmess)
        {
            if (id != sendmess.Id)
            {
                return BadRequest(new BaseResponse<SendmessModel>("400", "invalid_input_data", null));
            }

            _context.Entry(sendmess).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SendmessExists(id))
                {
                    return NotFound(new BaseResponse<SendmessModel>("404", "not_found", null));
                }
                else
                {
                    throw;
                }
            }

            return Ok(new BaseResponse<SendmessModel>("200", "success", sendmess));
        }

        [HttpPost]
        public async Task<ActionResult<SendmessModel>> PostSendmess(SendmessModel sendmess)
        {
            if (_context.Sendmesses == null)
            {
                return NotFound(new BaseResponse<SendmessModel>("404", "not_found", null));
            }
            _context.Sendmesses.Add(sendmess);
            await _context.SaveChangesAsync();

            return Ok(new BaseResponse<SendmessModel>("200", "success", sendmess));
        }

        // DELETE: api/Sendmess/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSendmess(int id)
        {
            if (_context.Sendmesses == null)
            {
                return NotFound(new BaseResponse<SendmessModel>("404", "not_found", null));
            }
            var sendmess = await _context.Sendmesses.FindAsync(id);
            if (sendmess == null)
            {
                return NotFound(new BaseResponse<SendmessModel>("404", "not_found", null));
            }

            _context.Sendmesses.Remove(sendmess);
            await _context.SaveChangesAsync();

            return Ok(new BaseResponse<SendmessModel>("200", "success", null));
        }

        private bool SendmessExists(int id)
        {
            return (_context.Sendmesses?.Any(e => e.Id == id)).GetValueOrDefault();
        }


    }
}

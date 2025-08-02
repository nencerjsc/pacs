using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class CurrencyController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CurrencyController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Currencies
        [HttpGet("GetAll")]
        public async Task<ActionResult<BaseResponseList<List<CurrencyModel>>>> GetCurrenciesModel(
                [FromQuery] string? name,
                [FromQuery] DateTime? startDate,
                [FromQuery] DateTime? endDate,
                [FromQuery] int pageNumber = 1,
                [FromQuery] int pageSize = 10

            )
        {
            if (_context.CurrenciesModel == null)
            {
                return NotFound(new BaseResponse<CurrencyModel>("404", "not_found", null));
            }

            var query = _context.CurrenciesModel.AsQueryable();
            var filter = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(name))
            {
                filter.Add(nameof(CurrencyModel.Name), name);
            }
            query = query.ApplyFilter(filter);

            if (startDate != null && endDate != null)
            {
                query = query.ApplyDateRangeFilter(c => c.CreatedAt ?? DateTime.Now, startDate, endDate);
            }

            int totalRecord = await query.CountAsync();
            int totalPage = (int)Math.Ceiling(totalRecord / (double)pageSize);

            var Curents = await query.ApplyPagination(pageNumber, pageSize).ToListAsync();
            var repone = new BaseResponseList<List<CurrencyModel>>("200", "Success", Curents, pageNumber, pageSize, totalPage);
            return Ok(repone);
        }

        // GET: api/Currencies/5
        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<CurrencyModel>> GetCurrenciesModel(int id)
        {
            if (_context.CurrenciesModel == null)
            {
                return NotFound(new BaseResponse<CurrencyModel>("404", "not_found", null));
            }
            var currenciesModel = await _context.CurrenciesModel.FindAsync(id);

            if (currenciesModel == null)
            {
                return NotFound(new BaseResponse<CurrencyModel>("404", "not_found", null));
            }

            return Ok(new BaseResponse<CurrencyModel>("200", "success", currenciesModel));
        }

        // PUT: api/Currencies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> PutCurrenciesModel(int id, CurrencyModel currenciesModel)
        {
            if (id != currenciesModel.Id)
            {
                return BadRequest(new BaseResponse<CurrencyModel>("400", "invalid_input_data", null));
            }

            _context.Entry(currenciesModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CurrenciesModelExists(id))
                {
                    return NotFound(new BaseResponse<CurrencyModel>("404", "not_found", null));
                }
                else
                {
                    throw;
                }
            }

            return Ok(new BaseResponse<CurrencyModel>("200", "Success", currenciesModel));
        }

        // POST: api/Currencies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Create")]
        public async Task<ActionResult<CurrencyModel>> PostCurrenciesModel(CurrencyModel currenciesModel)
        {
            if (_context.CurrenciesModel == null)
            {
                return NotFound(new BaseResponse<CurrencyModel>("404", "not_found", null));
            }
            _context.CurrenciesModel.Add(currenciesModel);
            await _context.SaveChangesAsync();

            return Ok(new BaseResponse<CurrencyModel>("200", "Success", currenciesModel));
        }

        // DELETE: api/Currencies/5
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteCurrenciesModel(int id)
        {
            if (_context.CurrenciesModel == null)
            {
                return NotFound(new BaseResponse<CurrencyModel>("404", "not_found", null));
            }
            var currenciesModel = await _context.CurrenciesModel.FindAsync(id);
            if (currenciesModel == null)
            {
                return NotFound(new BaseResponse<CurrencyModel>("404", "not_found", null));
            }

            _context.CurrenciesModel.Remove(currenciesModel);
            await _context.SaveChangesAsync();

            return Ok(new BaseResponse<CurrencyModel>("200", "Success", currenciesModel));
        }

        private bool CurrenciesModelExists(int id)
        {
            return (_context.CurrenciesModel?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

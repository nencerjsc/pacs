using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NencerApi.Helpers;
using NencerApi.Modules.SystemNc.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NencerCore.Modules.SystemNc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SettingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SettingController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Setting
        [HttpGet]
        public async Task<ActionResult<BaseResponseList<List<Setting>>>> GetSettings(
                    [FromQuery] DateTime? startDate,
                    [FromQuery] DateTime? endDate,
                    [FromQuery] string? key,
                    [FromQuery] int pageNumber = 1,
                    [FromQuery] int pageSize = 10)
        {
            if (_context.Settings == null)
            {
                return NotFound(new BaseResponse<Setting>("404", "not_found", null));
            }

            var query = _context.Settings.AsQueryable();

            var filters = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(key))
            {
                //filters.Add(nameof(Setting.Key), key);
                query = query.Where(x => (x.Key != null && x.Key.Contains(key)) || (x.Group != null && x.Group.Contains(key)));
            }

            query = query.ApplyFilter(filters);
            if (startDate != null && endDate != null)
            {
                query = query.ApplyDateRangeFilter(s => (DateTime)(s.CreatedAt ?? DateTime.Now), startDate, endDate);
            }

            int totalRecord = await query.CountAsync();
            int totalPage = (int)Math.Ceiling(totalRecord / (double)pageSize);
            var settings = await query.ApplyPagination(pageNumber, pageSize).ToListAsync();

            var response = new BaseResponseList<List<Setting>>("200", "success", settings, pageNumber, pageSize, totalPage);
            return Ok(response);
        }

        // GET: api/Setting/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Setting>> GetSetting(int id)
        {
            if (_context.Settings == null)
            {
                return NotFound(new BaseResponse<Setting>("404", "not_found", null));
            }
            var setting = await _context.Settings.FindAsync(id);

            if (setting == null)
            {
                return NotFound(new BaseResponse<List<Setting>>("404", "not_found", null));
            }

            return Ok(new BaseResponse<Setting>("200", "success", setting));
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutSetting(int id, Setting setting)
        {
            if (id != setting.Id)
            {
                return BadRequest(new BaseResponse<Setting>("400", "invalid_input_data", null));
            }

            _context.Entry(setting).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SettingExists(id))
                {
                    return NotFound(new BaseResponse<Setting>("404", "not_found", null));
                }
                else
                {
                    throw;
                }
            }

            return Ok(new BaseResponse<Setting>("200", "success", setting));
        }


        [HttpPost]
        public async Task<ActionResult<Setting>> PostSetting(Setting setting)
        {
            if (_context.Settings == null)
            {
                return NotFound(new BaseResponse<Setting>("404", "not_found", null));
            }
            _context.Settings.Add(setting);
            await _context.SaveChangesAsync();

            return Ok(new BaseResponse<Setting>("200", "success", setting));
        }

        // DELETE: api/Setting/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSetting(int id)
        {
            if (_context.Settings == null)
            {
                return NotFound(new BaseResponse<Setting>("404", "not_found", null));
            }
            var setting = await _context.Settings.FindAsync(id);
            if (setting == null)
            {
                return NotFound(new BaseResponse<Setting>("404", "not_found", null));
            }

            _context.Settings.Remove(setting);
            await _context.SaveChangesAsync();

            return Ok(new BaseResponse<Setting>("200", "success", null));
        }

        private bool SettingExists(int id)
        {
            return (_context.Settings?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

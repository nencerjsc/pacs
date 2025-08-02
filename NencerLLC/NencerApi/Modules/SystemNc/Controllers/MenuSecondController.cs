using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NencerApi.Helpers;
using NencerApi.Modules.SystemNc.Model;

namespace NencerCore.Modules.SystemNc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuSecondController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MenuSecondController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/MenuSecond/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MenuSecond>> GetMenuSecond(int id)
        {
            var menuSecond = await _context.MenuSeconds.FirstOrDefaultAsync(m => m.Id == id);

            if (menuSecond == null)
            {
                return NotFound(new BaseResponse<MenuSecond>("404", "not_found", null));
            }
            return Ok(new BaseResponse<MenuSecond>("200", "success", menuSecond));
        }

        [HttpPost]
        public async Task<ActionResult<MenuSecond>> PostMenuSecond(MenuSecond menuSecond)
        {
            if (menuSecond.FirstId == null)
            {
                return BadRequest(new BaseResponse<MenuSecond>("400", "first_id_required", null));
            }
            _context.MenuSeconds.Add(menuSecond);
            await _context.SaveChangesAsync();
            return Ok(new BaseResponse<MenuSecond>("200", "success", menuSecond));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMenuSecond(int id, [FromBody] MenuSecondUpdate dto)
        {
            var entity = await _context.MenuSeconds.FindAsync(id);
            if (entity == null)
            {
                return NotFound(new BaseResponse<MenuSecond>("404", "not_found", null));
            }
            entity.FirstId = dto.FirstId;
            // Chỉ cập nhật các trường có giá trị
            if (!string.IsNullOrEmpty(dto.Name)) entity.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Description)) entity.Description = dto.Description;
            if (!string.IsNullOrEmpty(dto.Image)) entity.Image = dto.Image;
            if (!string.IsNullOrEmpty(dto.FaIcon)) entity.FaIcon = dto.FaIcon;
            if (!string.IsNullOrEmpty(dto.Url)) entity.Url = dto.Url;

            if (dto.Sort.HasValue) entity.Sort = dto.Sort.Value;
            if (dto.Status.HasValue) entity.Status = dto.Status.Value;

            await _context.SaveChangesAsync();
            return Ok(new BaseResponse<MenuSecond>("200", "success", entity));
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenuSecond(int id)
        {
            var menuSecond = await _context.MenuSeconds.FirstOrDefaultAsync(m => m.Id == id);
            if (menuSecond == null)
            {
                return NotFound(new BaseResponse<MenuSecond>("404", "not_found", null));
            }

            _context.MenuSeconds.Remove(menuSecond);
            await _context.SaveChangesAsync();

            return Ok(new BaseResponse<MenuSecond>("200", "success", null));
        }

        private bool MenuSecondExists(int id)
        {
            return _context.MenuSeconds.Any(e => e.Id == id);
        }
    }
}

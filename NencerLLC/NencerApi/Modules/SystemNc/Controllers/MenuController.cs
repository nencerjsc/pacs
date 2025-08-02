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
    public class MenuController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MenuController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Menu
        [HttpGet]
        public async Task<ActionResult<BaseResponseList<List<MenuFirstDto>>>> GetMenus()
        {
            var menus = await _context.MenuFirsts.OrderBy(m => m.Sort)
                .Select(m => new MenuFirstDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    Image = m.Image,
                    ImageBg = m.ImageBg,
                    FaIcon = m.FaIcon,
                    Url = m.Url,
                    Sort = m.Sort,
                    Status = m.Status,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt,
                    MenuSeconds = _context.MenuSeconds
                        .Where(s => s.FirstId == m.Id)
                        .OrderBy(s => s.Sort)
                        .Select(s => new MenuSecondDto
                        {
                            Id = s.Id,
                            Name = s.Name,
                            Description = s.Description,
                            Image = s.Image,
                            FaIcon = s.FaIcon,
                            Url = s.Url,
                            Sort = s.Sort,
                            Status = s.Status,
                            CreatedAt = s.CreatedAt,
                            UpdatedAt = s.UpdatedAt
                        }).ToList()
                })
                .ToListAsync();

            return Ok(new BaseResponseList<List<MenuFirstDto>>("200", "success", menus));
        }

        // GET: api/Menu/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MenuDto>> GetMenu(int id)
        {
            var first = await _context.MenuFirsts.FirstOrDefaultAsync(m => m.Id == id);
            if (first == null)
            {
                return NotFound(new BaseResponse<MenuDto>("404", "not_found", null));
            }

            var seconds = _context.MenuSeconds.Where(x => x.FirstId == id).ToList();
            var data = new MenuDto();
            data.MenuFirst = first;
            data.MenuSeconds = seconds;

            return Ok(new BaseResponse<MenuDto>("200", "success", data));
        }

        [HttpPost]
        public async Task<ActionResult<MenuFirst>> PostMenu(MenuFirst menu)
        {
            _context.MenuFirsts.Add(menu);
            await _context.SaveChangesAsync();
            return Ok(new BaseResponse<MenuFirst>("200", "success", menu));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMenu(int id, [FromBody] MenuFirstUpdate dto)
        {
            var entity = await _context.MenuFirsts.FindAsync(id);
            if (entity == null)
            {
                return NotFound(new BaseResponse<MenuFirst>("404", "not_found", null));
            }

            // Chỉ cập nhật các trường được truyền lên
            if (!string.IsNullOrEmpty(dto.Name)) entity.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Description)) entity.Description = dto.Description;
            if (!string.IsNullOrEmpty(dto.Image)) entity.Image = dto.Image;
            if (!string.IsNullOrEmpty(dto.ImageBg)) entity.ImageBg = dto.ImageBg;
            if (!string.IsNullOrEmpty(dto.FaIcon)) entity.FaIcon = dto.FaIcon;
            if (!string.IsNullOrEmpty(dto.Url)) entity.Url = dto.Url;
            if (dto.TypeId.HasValue) entity.TypeId = dto.TypeId.Value;
            if (dto.Sort.HasValue) entity.Sort = dto.Sort.Value;
            if (dto.Status.HasValue) entity.Status = dto.Status.Value;

            await _context.SaveChangesAsync();
            return Ok(new BaseResponse<MenuFirst>("200", "success", entity));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            var menu = await _context.MenuFirsts.FirstOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                return NotFound(new BaseResponse<MenuFirst>("404", "not_found", null));
            }

            int count = await _context.MenuSeconds.Where(x => x.Id == id).CountAsync();
            if (count > 0)
            {
                return Ok(new BaseResponse<MenuFirst>("500", "delete_child_menu_first", null));
            }

            _context.MenuFirsts.Remove(menu);
            await _context.SaveChangesAsync();

            return Ok(new BaseResponse<MenuFirst>("200", "success", null));
        }

        private bool MenuExists(int id)
        {
            return _context.MenuFirsts.Any(e => e.Id == id);
        }
    }
}

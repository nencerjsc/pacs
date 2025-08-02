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
    public class FileServerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FileServerController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<BaseResponseList<List<FileServer>>>> GetFileServers(
            [FromQuery] string? serverName,
            [FromQuery] string? type,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (_context.FileServers == null)
                return NotFound(new BaseResponse<FileServer>("404", "not_found", null));

            var query = _context.FileServers.AsQueryable();
            var filter = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(serverName))
                filter.Add(nameof(FileServer.ServerName), serverName);
            if (!string.IsNullOrEmpty(type))
                filter.Add(nameof(FileServer.Type), type);

            query = query.ApplyFilter(filter);

            int totalRecord = await query.CountAsync();
            int totalPage = (int)Math.Ceiling(totalRecord / (double)pageSize);
            var items = await query.ApplyPagination(pageNumber, pageSize).ToListAsync();

            return Ok(new BaseResponseList<List<FileServer>>("200", "Success", items, pageNumber, pageSize, totalPage));
        }

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<FileServer>> GetFileServer(int id)
        {
            var server = await _context.FileServers.FindAsync(id);
            if (server == null)
                return NotFound(new BaseResponse<FileServer>("404", "not_found", null));

            return Ok(new BaseResponse<FileServer>("200", "Success", server));
        }

        [HttpPost("Create")]
        public async Task<ActionResult<FileServer>> CreateFileServer(FileServer server)
        {
            _context.FileServers.Add(server);
            await _context.SaveChangesAsync();

            return Ok(new BaseResponse<FileServer>("200", "Success", server));
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateFileServer(int id, FileServer server)
        {
            if (id != server.Id)
                return BadRequest(new BaseResponse<FileServer>("400", "invalid_input_data", null));

            _context.Entry(server).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FileServerExists(id))
                    return NotFound(new BaseResponse<FileServer>("404", "not_found", null));
                else
                    throw;
            }

            return Ok(new BaseResponse<FileServer>("200", "Success", server));
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteFileServer(int id)
        {
            var server = await _context.FileServers.FindAsync(id);
            if (server == null)
                return NotFound(new BaseResponse<FileServer>("404", "not_found", null));

            _context.FileServers.Remove(server);
            await _context.SaveChangesAsync();

            return Ok(new BaseResponse<FileServer>("200", "Success", server));
        }

        private bool FileServerExists(int id)
        {
            return _context.FileServers.Any(e => e.Id == id);
        }
    }
}

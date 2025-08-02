using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NencerApi.Modules.SystemNc.Model.DigiSign;
using NencerCore;

namespace NencerApi.Modules.SystemNc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DigiSignController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly DigitalSignFactory _signFactory;

        public DigiSignController(AppDbContext context, DigitalSignFactory signFactory)
        {
            _context = context;
            _signFactory = signFactory;
        }

        [HttpPost("sign")]
        public async Task<IActionResult> SignFile(
            IFormFile file,
            [FromQuery] string providerCode,
            [FromQuery] string dataType // "pdf", "xml"
        )
        {
            if (file == null || file.Length == 0)
                return BadRequest("Không có file được gửi.");

            var providerEntity = await _context.DigiSignProviders
                .Where(x => x.Code == providerCode && x.Status == 1)
                .FirstOrDefaultAsync();

            if (providerEntity == null)
                return BadRequest("Nhà cung cấp không tồn tại hoặc đang bị vô hiệu.");

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            byte[] rawData = ms.ToArray();

            try
            {
                var provider = _signFactory.GetProvider(providerCode);
                var signedData = provider.SignData(rawData, dataType, providerEntity.ConfigJson);

                string contentType = dataType switch
                {
                    "pdf" => "application/pdf",
                    "xml" => "application/xml",
                    _ => "application/octet-stream"
                };

                return File(signedData, contentType, $"signed.{dataType}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ký số: {ex.Message}");
            }
        }
    }
}

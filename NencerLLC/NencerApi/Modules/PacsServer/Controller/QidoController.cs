using System.Data;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using FellowOakDicom;
using FellowOakDicom.Network;
using Serilog;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.Security.Claims;
using NencerCore;
using NencerApi.Modules.PacsServer.Helpers;


namespace NencerApi.Modules.PacsServer.Controller
{
    //[Authorize]
    [ApiController]
    [Route("studies")]
    public class QidoController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;

        public QidoController(AppDbContext context, IConfiguration config)
        {
            _config = config;
            _context = context;
        }
        
        [HttpGet]        
        public async Task<IActionResult> GetStudies(
            [FromQuery(Name = "00100020")] string patientId = "",
            [FromQuery(Name = "limit")] int limit = 100,
            [FromQuery(Name = "offset")] int offset = 0,
            [FromQuery(Name = "fuzzymatching")] bool fuzzymatching = false,
            [FromQuery(Name = "includeField")] string includeField = "",
            [FromQuery(Name = "StudyInstanceUID")] string? StudyInstanceUID = "")
        {

            var query = _context.DicomStudies.AsQueryable();

            if (!string.IsNullOrEmpty(patientId))
            {
                // Tìm theo PatientID
                if (fuzzymatching)
                    query = query.Where(s => s.PatientID == patientId);                
                else
                    query = query.Where(s => s.PatientID.Contains(patientId));
            }
            

            if (!string.IsNullOrWhiteSpace(StudyInstanceUID))
                query = query.Where(s => s.StudyInstanceUID == StudyInstanceUID);

            // Lấy tổng số bản ghi (nếu cần phân trang nâng cao)
            var total = await query.CountAsync();

            // Áp dụng phân trang
            var studies = await query
                .OrderByDescending(s => s.StudyDate)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();

            var result = studies.Select(DicomStudyMapperHelper.ToDicomJson).ToList();
            return Ok(result);
           
        }

        [HttpPost]
        [Consumes("multipart/related")]
        public async Task<IActionResult> UploadDicomFile()
        {
            // ✅ Kiểm tra Content-Type hợp lệ
            if (!MediaTypeHeaderValue.TryParse(Request.ContentType, out var mediaTypeHeader))
                return BadRequest("Content-Type không hợp lệ");

            var boundary = HeaderUtilities.RemoveQuotes(mediaTypeHeader.Boundary).Value;
            if (string.IsNullOrWhiteSpace(boundary))
                return BadRequest("Không tìm thấy boundary");

            var reader = new MultipartReader(boundary, Request.Body);
            var savedInstances = new List<string>();

            MultipartSection? section;
            while ((section = await reader.ReadNextSectionAsync()) != null)
            {
                // ✅ Kiểm tra MIME Type
                if (!string.Equals(section.ContentType, "application/dicom", StringComparison.OrdinalIgnoreCase))
                    continue;

                try
                {
                    // ✅ Đọc section.Body vào MemoryStream để tránh lỗi AllowSynchronousIO
                    using var ms = new MemoryStream();
                    await section.Body.CopyToAsync(ms);
                    ms.Seek(0, SeekOrigin.Begin);

                    // ✅ Đọc file DICOM
                    var dicomFile = await DicomFile.OpenAsync(ms);
                    var sopInstanceUID = dicomFile.Dataset.GetSingleValueOrDefault<string>(DicomTag.SOPInstanceUID, Guid.NewGuid().ToString());

                    // ✅ Lưu file DICOM
                    var folder = Path.Combine("DicomFiles", DateTime.UtcNow.ToString("yyyyMMdd"));
                    Directory.CreateDirectory(folder);
                    var savePath = Path.Combine(folder, $"{sopInstanceUID}.dcm");

                    await dicomFile.SaveAsync(savePath);
                    savedInstances.Add(sopInstanceUID);
                }
                catch (Exception ex)
                {
                    return BadRequest(new
                    {
                        error = "Không thể đọc/lưu file DICOM",
                        message = ex.Message
                    });
                }
            }

            return Ok(new
            {
                message = "Upload DICOM thành công",
                count = savedInstances.Count,
                instances = savedInstances
            });
        }

    
    }
}

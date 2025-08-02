using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NencerApi.Modules.SystemNc.Model;
using NencerCore;
using System.Net;

namespace NencerApi.Modules.SystemNc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FtpController : ControllerBase
    {

        private readonly AppDbContext _context;

        public FtpController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("upload-ftp")]
        public async Task<IActionResult> UploadToFtp(IFormFile file, string ftpFilePath, int fileServerId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File không hợp lệ.");

            var fileServer = await _context.FileServers
                .Where(x => x.Id == fileServerId && x.Status == 1 && x.Type == "FTP")
                .FirstOrDefaultAsync();

            if (fileServer == null)
                return NotFound("Không tìm thấy máy chủ FTP.");

            try
            {
                // Tạo thư mục theo cấu trúc: năm/tháng/ngày
                string currentDateFolder = DateTime.Now.ToString("yyyy/MM/dd");
                string safeFileName = Path.GetFileName(file.FileName);

                // Kết hợp thêm đường dẫn con nếu cần
                string subFolderPath = $"{currentDateFolder}/{ftpFilePath}".Replace("\\", "/").TrimEnd('/');

                // Tạo thư mục nếu chưa có
                await CreateFtpDirectoryIfNotExist(subFolderPath, fileServer);

                // Đường dẫn đầy đủ để upload file
                string baseUrl = fileServer.HostName.StartsWith("ftp://")
                    ? fileServer.HostName
                    : $"ftp://{fileServer.HostName}";
                string fullFtpPath = $"{baseUrl}/{subFolderPath}/{safeFileName}".Replace("\\", "/");
                fullFtpPath = Uri.EscapeUriString(fullFtpPath);

                // Upload file
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(fullFtpPath);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(fileServer.UserName, fileServer.Password);
                request.UseBinary = true;
                request.UsePassive = true;

                using (var stream = file.OpenReadStream())
                using (var requestStream = request.GetRequestStream())
                {
                    await stream.CopyToAsync(requestStream);
                }

                using (var response = (FtpWebResponse)request.GetResponse())
                {
                    return Ok(new
                    {
                        message = "Upload thành công",
                        filePath = $"{subFolderPath}/{safeFileName}",
                        server_id = fileServerId,
                        status = response.StatusDescription
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi upload FTP: " + ex.Message);
            }
        }

        private async Task CreateFtpDirectoryIfNotExist(string ftpFolderPath, FileServer fileServer)
        {
            string[] subDirs = ftpFolderPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            string currentPath = fileServer.HostName.StartsWith("ftp://")
                ? fileServer.HostName
                : $"ftp://{fileServer.HostName}";

            foreach (var dir in subDirs)
            {
                currentPath = $"{currentPath}/{dir}";
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(currentPath);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.Credentials = new NetworkCredential(fileServer.UserName, fileServer.Password);
                request.UseBinary = true;
                request.UsePassive = true;

                try
                {
                    using var resp = (FtpWebResponse)await request.GetResponseAsync();
                }
                catch (WebException ex)
                {
                    var response = (FtpWebResponse)ex.Response;
                    if (response.StatusCode != FtpStatusCode.ActionNotTakenFileUnavailable)
                        throw;
                }
            }
        }

        [HttpGet("download-ftp")]
        public async Task<IActionResult> DownloadFromFtp(string ftpFilePath, int fileServerId)
        {
            var fileServer = await _context.FileServers
                .Where(x => x.Id == fileServerId && x.Status == 1 && x.Type == "FTP")
                .FirstOrDefaultAsync();

            if (fileServer == null)
                return NotFound("Không tìm thấy máy chủ FTP.");

            try
            {
                string baseUrl = fileServer.HostName.StartsWith("ftp://")
                    ? fileServer.HostName
                    : $"ftp://{fileServer.HostName}";

                string fullFtpPath = $"{baseUrl}/{ftpFilePath}".Replace("\\", "/");
                fullFtpPath = Uri.EscapeUriString(fullFtpPath);

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(fullFtpPath);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(fileServer.UserName, fileServer.Password);
                request.UseBinary = true;
                request.UsePassive = true;

                using var response = (FtpWebResponse)await request.GetResponseAsync();
                using var responseStream = response.GetResponseStream();
                using var ms = new MemoryStream();
                await responseStream.CopyToAsync(ms);
                ms.Position = 0;

                string fileName = Path.GetFileName(ftpFilePath);
                string contentType = "application/octet-stream"; // Có thể dùng logic MIME type nếu cần

                return File(ms.ToArray(), contentType, fileName);
            }
            catch (WebException ex)
            {
                var response = ex.Response as FtpWebResponse;
                return StatusCode(500, $"Lỗi tải file: {ex.Message} ({response?.StatusDescription})");
            }
        }


        [HttpGet("view-ftp")]
        public async Task<IActionResult> ViewFileFromFtp(string ftpFilePath, int fileServerId)
        {
            var fileServer = await _context.FileServers
                .Where(x => x.Id == fileServerId && x.Status == 1 && x.Type == "FTP")
                .FirstOrDefaultAsync();

            if (fileServer == null)
                return NotFound("Không tìm thấy máy chủ FTP.");

            try
            {
                string baseUrl = fileServer.HostName.StartsWith("ftp://")
                    ? fileServer.HostName
                    : $"ftp://{fileServer.HostName}";

                string fullFtpPath = $"{baseUrl}/{ftpFilePath}".Replace("\\", "/");
                fullFtpPath = Uri.EscapeUriString(fullFtpPath);

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(fullFtpPath);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(fileServer.UserName, fileServer.Password);
                request.UseBinary = true;
                request.UsePassive = true;

                using var response = (FtpWebResponse)await request.GetResponseAsync();
                using var responseStream = response.GetResponseStream();
                using var ms = new MemoryStream();
                await responseStream.CopyToAsync(ms);
                ms.Position = 0;

                string fileName = Path.GetFileName(ftpFilePath);
                string contentType = GetMimeType(fileName);

                // Mở file inline trên trình duyệt (PDF/JPG/PNG...)
                return File(ms.ToArray(), contentType, fileName, enableRangeProcessing: true);
            }
            catch (WebException ex)
            {
                var response = ex.Response as FtpWebResponse;
                return StatusCode(500, $"Lỗi tải file: {ex.Message} ({response?.StatusDescription})");
            }
        }

        private string GetMimeType(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();

            return extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".svg" => "image/svg+xml",
                _ => "application/octet-stream"
            };
        }


    }
}

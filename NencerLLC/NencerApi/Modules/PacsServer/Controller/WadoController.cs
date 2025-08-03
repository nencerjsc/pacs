using FellowOakDicom.Imaging.Codec;
using FellowOakDicom.Imaging;
using FellowOakDicom;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using NencerCore;
using NencerApi.Modules.PacsServer.Model;
using Microsoft.EntityFrameworkCore;
using NencerApi.Modules.PacsServer.Helpers;
using NencerApi.Modules.PacsServer.Service;

namespace NencerApi.Modules.PacsServer.Controller
{
    [ApiController]
    [Route("studies")]
    public class WadoController : ControllerBase
    {
        private readonly DicomLocalService _service;
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public WadoController(AppDbContext context , IConfiguration config)
        {
            _config = config;
            _context = context;
        }

        [HttpGet("{studyUID}/series")]
        public async Task<IActionResult> GetSeries(string studyUID)
        {

            // 🔍 Lấy thông tin study tương ứng
            var study = await _context.DicomStudies
                .FirstOrDefaultAsync(s => s.StudyInstanceUID == studyUID);

            if (study == null)
                return NotFound("Study không tồn tại.");

            // ✅ Lấy series nếu quyền hợp lệ
            var seriesList = await _context.DicomSeries
                .Where(s => s.StudyInstanceUID == studyUID)
                .ToListAsync();

            var result = seriesList
                .Select(DicomSeriesMapperHelper.ToDicomJson)
                .ToList();

            return Ok(result);
        }

        [HttpGet("{studyUID}/series/{seriesUID}/metadata")]
        public async Task<IActionResult> GetInstanceMetadata(string studyUID, string seriesUID)
        {
            try
            {

                // 🔍 Lấy thông tin study để xác định quyền truy cập
                var study = await _context.DicomStudies
                    .FirstOrDefaultAsync(s => s.StudyInstanceUID == studyUID);

                if (study == null)
                    return NotFound("Study không tồn tại.");


                // ✅ Lấy các instance trong series
                var instances = await _context.DicomInstances
                    .Where(i => i.StudyInstanceUID == studyUID && i.SeriesInstanceUID == seriesUID)
                    .ToListAsync();

                if (!instances.Any())
                    return NotFound("Không tìm thấy instance trong series.");

                var allMetadata = new List<object>();

                foreach (var instance in instances)
                {
                    var tags = await _context.DicomTags
                        .Where(t => t.SOPInstanceUID == instance.SOPInstanceUID)
                        .ToListAsync();

                    var tree = BuildDicomTagTree(tags, instance.SOPInstanceUID);

                    var metadata = new Dictionary<string, object>();
                    foreach (var tagItem in tree)
                    {
                        var tagJson = tagItem.ToDicomJson();
                        foreach (var kv in tagJson)
                        {
                            metadata[kv.Key] = kv.Value;
                        }
                    }

                    allMetadata.Add(metadata);
                }

                return Ok(allMetadata);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message} - {ex.StackTrace}");
            }

        }

        private List<DicomTagItemModel> BuildDicomTagTree(List<DicomTagItemModel> flatTags, string sopInstanceUID)
        {
            var lookup = flatTags.ToLookup(x => x.ParentTag);

            List<DicomTagItemModel> Build(string parentTag)
            {
                var children = lookup[parentTag].ToList();

                foreach (var child in children)
                {
                    if (child.ValueType == "SQ")
                    {
                        // Nếu VR = SQ thì tiếp tục tìm các con bên trong
                        child.Children = Build(child.Name);
                    }
                }

                return children;
            }

            return Build(string.Empty); // Bắt đầu từ root (không có ParentTag)
        }


        [HttpGet("{studyUID}/series/{seriesUID}/instances/{instanceUID}/frames/{frameNumber}")]
        public async Task<IActionResult> GetFrame(string studyUID, string seriesUID, string instanceUID, int frameNumber)
        {

            // 🔍 Xác định study để kiểm tra PatientID
            var study = await _context.DicomStudies
                .FirstOrDefaultAsync(s => s.StudyInstanceUID == studyUID);

            if (study == null)
                return NotFound("Study không tồn tại.");

            // Tìm đường dẫn file ảnh DICOM theo UID
            var instance = await _context.DicomInstances
                .Where(i => i.StudyInstanceUID == studyUID &&
                            i.SeriesInstanceUID == seriesUID &&
                            i.SOPInstanceUID == instanceUID)
                .FirstOrDefaultAsync();

            if (instance == null || string.IsNullOrEmpty(instance.FilePath))
                return NotFound("DICOM instance not found or file path missing.");

            try
            {
                var storageBasePath = _config["Storage:BasePath"];
                var storageFullPath = Path.Combine(storageBasePath, instance.FilePath);
                if (!System.IO.File.Exists(storageFullPath))
                    return NotFound("DICOM file not found.");

                var dicomFile = await DicomFile.OpenAsync(storageFullPath);


                var transcoder = new DicomTranscoder(
                    dicomFile.FileMetaInfo.TransferSyntax,
                    DicomTransferSyntax.ExplicitVRLittleEndian // hoặc DicomTransferSyntax.ImplicitVRLittleEndian
                );
                dicomFile = transcoder.Transcode(dicomFile);


                var pixelData = DicomPixelData.Create(dicomFile.Dataset);

                if (frameNumber < 1 || frameNumber > pixelData.NumberOfFrames)
                    return BadRequest("Invalid frame number.");

                var frame = pixelData.GetFrame(frameNumber - 1);

                return CreateMultipartResponse(frame.Data, dicomFile.FileMetaInfo.TransferSyntax.UID.UID);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error reading DICOM image: {ex.Message}");
            }
        }

        private IActionResult CreateMultipartResponse(byte[] imageData, string transferSyntax)
        {
            var boundary = $"BOUNDARY_{Guid.NewGuid()}";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.ASCII, leaveOpen: true);

            // Start multipart
            writer.WriteLine($"--{boundary}");
            //writer.WriteLine("Content-Type: application/octet-stream");
            //writer.WriteLine($"Content-Transfer-Encoding: binary");
            //writer.WriteLine($"Content-Location: frame");
            writer.WriteLine($"Content-Type: image/jls; transfer-syntax={transferSyntax}");
            writer.WriteLine();
            writer.Flush();

            // Write raw image data
            ms.Write(imageData, 0, imageData.Length);
            ms.Flush(); // Bắt buộc flush sau khi write

            // Write end boundary
            var endBoundary = Encoding.ASCII.GetBytes($"\r\n--{boundary}--\r\n");
            ms.Write(endBoundary, 0, endBoundary.Length);

            ms.Position = 0;
            Response.ContentType = $"multipart/related; boundary={boundary}";
            Response.Headers["Accept-Ranges"] = "bytes";

            return File(ms, Response.ContentType);
        }

    }
}

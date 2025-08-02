using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Management;
using System.Net.NetworkInformation;


namespace NencerApi.Modules.SystemNc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LicenseController : ControllerBase
    {
        //1024 private key
        [HttpGet("Generate")]
        public string GenerateLicense(string productId, string customerId, string expiryDate, string licInfo)
        {
            // Dữ liệu license dưới dạng JSON
            var licenseData = new
            {
                ProductId = productId,
                CustomerId = customerId,
                ExpiryDate = expiryDate,
                LicInfo = licInfo
            };

            var licenseJson = JsonConvert.SerializeObject(licenseData);
            var licenseBytes = Encoding.UTF8.GetBytes(licenseJson);

            // Mã hóa bằng AES
            var encryptedLicense = EncryptLicense(licenseBytes);

            // Trả về chuỗi mã hóa Base64
            return Convert.ToBase64String(encryptedLicense);
        }


        private static byte[] EncryptLicense(byte[] data)
        {
            var key = Encoding.UTF8.GetBytes("BanquyenNencerVn"); // 16 bytes cho AES-128
            var iv = Encoding.UTF8.GetBytes("BanquyenNencerCo"); // 16 bytes cho IV

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    return encryptor.TransformFinalBlock(data, 0, data.Length);
                }
            }
        }


        [HttpGet("download-license")]
        public IActionResult DownloadLicense()
        {
            var filePath = "";
            var fileBytes = System.IO.File.ReadAllBytes(filePath); // Đọc file thành byte array
            var fileName = "license.lic";
            return File(fileBytes, "application/octet-stream", fileName); // Trả file về client
        }


        [HttpGet("machine-info")]
        public IActionResult GetMachineDetails()
        {
            var details = new MachineDetails
            {
                MachineName = Environment.MachineName,
                MacAddress = GetMacAddress(),
                Uuid = GetUuid(),
                OsPlatform = Environment.OSVersion.Platform.ToString()
            };

            return Ok(details);
        }


        private static string GetMacAddress()
        {
            try
            {
                return NetworkInterface
                    .GetAllNetworkInterfaces()
                    .FirstOrDefault(ni => ni.OperationalStatus == OperationalStatus.Up && ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    ?.GetPhysicalAddress()
                    .ToString() ?? "Unknown";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        private static string GetUuid()
        {
            try
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    // Windows: Sử dụng System.Management
                    var searcher = new ManagementObjectSearcher("select * from Win32_ComputerSystemProduct");
                    foreach (var obj in searcher.Get())
                    {
                        return obj["UUID"]?.ToString() ?? "Unknown";
                    }
                }
                else if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    // Linux/macOS: Đọc từ /etc/machine-id
                    return ExecuteCommand("cat", "/etc/machine-id").Trim();
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }

            return "Unknown";
        }

        private static string ExecuteCommand(string command, string arguments)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = command,
                        Arguments = arguments,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return result;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }



    }

    public class MachineDetails
    {
        public string MachineName { get; set; }
        public string MacAddress { get; set; }
        public string Uuid { get; set; }
        public string OsPlatform { get; set; }
    }


}

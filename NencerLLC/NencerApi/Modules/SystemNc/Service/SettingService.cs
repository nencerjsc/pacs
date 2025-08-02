using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using NencerApi.Helpers;
using NencerApi.Modules.SystemNc.Model;
using NencerCore;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NencerApi.Modules.SystemNc.Service
{
    public class SettingService
    {
        private readonly AppDbContext _context;

        public SettingService(AppDbContext context)
        {
            _context = context;
        }

        public static string get_menu_type = "";
        public static string ticket_queue_setting = "";

        public async Task<string> GetMenuType()
        {
            if (string.IsNullOrEmpty(get_menu_type))
            {
                get_menu_type = (await GetSettingByKey("get_menu_type"))?.Data?.Value;
            }

            if (string.IsNullOrEmpty(get_menu_type))
            {
                get_menu_type = "0";
            }

            return get_menu_type;
        }

        public async Task<string> GetTicketQueueSetting()
        {
            if (string.IsNullOrEmpty(ticket_queue_setting))
            {
                ticket_queue_setting = (await GetSettingByKey("ticket_queue_setting"))?.Data?.Value;
            }

            if (string.IsNullOrEmpty(ticket_queue_setting))
            {
                ticket_queue_setting = "0";
            }

            return ticket_queue_setting;
        }

        public async Task<BaseResponse<Setting?>> GetSettingByKey(string key)
        {
            try
            {
                var data = await _context.Settings.FirstOrDefaultAsync(x => x.Key.ToLower() == key.ToLower().Trim());

                if (data == null)
                {
                    return new ErrorResponse<Setting?>("setting_not_found");
                }
                return new BaseResponse<Setting?> { Data = data };
            }
            catch (Exception ex)
            {
                return new ExceptionErrorResponse<Setting?>(ex, "FindSettingByKey");
            }
        }

        public async Task<BaseResponse<List<Setting>?>> GetListSettingByGroup(string group)
        {
            try
            {
                var list = _context.Settings.Where(x => x.Group == group).ToList();
                if (list == null)
                {
                    return new ErrorResponse<List<Setting>?>("settings_not_found");
                }

                return new BaseResponse<List<Setting>?>(list);
            }
            catch (Exception ex)
            {
                return new ExceptionErrorResponse<List<Setting>?>(ex, "GetListSettingByGroup");
            }
        }


        public bool ValidateLicense()
        {
            var filePath = Path.Combine(@"C:\NencerLLC", "License", "license.lic");
            if (!System.IO.File.Exists(filePath))
            {
                return false;
            }

            var encryptedLicense = System.IO.File.ReadAllText(filePath);

            var encryptedBytes = Convert.FromBase64String(encryptedLicense);
            var licenseBytes = DecryptLicense(encryptedBytes);

            // Chuyển đổi dữ liệu license thành JSON
            var licenseJson = Encoding.UTF8.GetString(licenseBytes);
            var licenseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(licenseJson);

            // Lấy ngày hết hạn
            if (licenseData != null && licenseData.ContainsKey("ExpiryDate"))
            {
                var expiryDate = DateTime.Parse(licenseData["ExpiryDate"]);
                return expiryDate >= DateTime.UtcNow;
            }

            return false;
        }

        private static byte[] DecryptLicense(byte[] encryptedData)
        {
            var key = Encoding.UTF8.GetBytes("BanquyenNencerVn"); // 16 bytes cho AES-128
            var iv = Encoding.UTF8.GetBytes("BanquyenNencerCo"); // 16 bytes cho IV

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    return decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                }
            }
        }

    }
}

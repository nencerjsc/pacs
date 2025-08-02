
using Microsoft.EntityFrameworkCore;
using NencerApi.Modules.SystemNc.Model;
using NencerCore;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace NencerApi.Helpers
{
    public static class Common
    {

        private static AppDbContext _context;
        public static void SetContext(AppDbContext context)
        {
            _context = context;
        }

        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }

        public static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public static void dd(this object obj)
        {
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            Debug.WriteLine(json);
            Console.WriteLine(json);
        }


        public static string getSiteInfo(string key)
        {
            var b = key;
            if (_context == null)
            {
                throw new Exception("_context is null. Ensure it is properly initialized.");
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));
            }
            if (key == null)
            {
                var a = 15;
                return "15";
            }
            Setting setting = _context.Settings.Where(s => s.Key == key).FirstOrDefault();
            if (setting != null) {
                return setting.Value;
            }
            else
            {
                return null;
            }

        }

       
    }
}

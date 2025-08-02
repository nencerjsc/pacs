using Hl7.FhirPath.Sprache;
using Microsoft.SqlServer.Server;
using Microsoft.VisualBasic;
using System.Globalization;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Nencer.Helpers
{
    public class ApiHelper
    {
        private static readonly Random random = new Random();
        public static string FormatDate(string format)
        {
            // Pad with zeroes on the left
            return DateTime.Now.ToString(format);//20230306
        }

        public static string FormatDate(string input, string format)
        {
            if (string.IsNullOrEmpty(input)) return "";
            try
            {
                DateTime dt = DateTime.ParseExact(
                input,
                format,
                CultureInfo.InvariantCulture);
                return dt.ToString(format);//dd/MM/yyyy
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string FormatDate(DateTime? dt, string format)
        {
            if (dt == null) return "";
            return dt.Value.ToString(format);//dd/MM/yyyy
        }

        public static string GenerateExamOrdinal(int num, int max)
        {
            // Pad with zeroes on the left
            string paddedNumber = num.ToString().PadLeft(max, '0');
            return DateTime.Now.ToString("yyyyMMdd") + paddedNumber; //2023101700001
        }

        public static string GetBarCode(int num, int max)
        {
            string paddedNumber = num.ToString().PadLeft(max, '0');
            return paddedNumber; //00001
        }

        public static int GetRandomNumber(int min, int max)
        {
            lock (random) // synchronize
            {
                return random.Next(min, max);
            }
        }

        public static DateTime? SafeToDate2(string? obj)
        {
            if (obj == null) return null;
            DateTime result;
            DateTime.TryParseExact(obj, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out result);
            return result;
        }

        public static string GenerateCheckinTakeNumber(int num, string key)
        {
            string paddedNumber = num.ToString().PadLeft(6, '0');
            return key + DateTime.Now.ToString("yyyyMMdd") + paddedNumber;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string GetIP()
        {
            string hostName = Dns.GetHostName();
            IPAddress[] localIPs = Dns.GetHostAddresses(hostName);

            IPAddress localIPv4 = localIPs.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

            if (localIPv4 != null)
            {
                return localIPv4.ToString();
            }
            else
            {
                return Guid.NewGuid().ToString();
            }
        }

    }
}

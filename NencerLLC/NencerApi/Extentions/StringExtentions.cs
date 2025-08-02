using System.Drawing;
using ZXing.Common;
using ZXing;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace NencerApi.Extentions
{
    public static class StringExtentions
    {
        /// <summary>
        /// Lưu chuỗi Base64 thành file trên hệ thống.
        /// </summary>
        /// <param name="base64String">Chuỗi Base64 cần lưu thành file.</param>
        /// <param name="filePath">Đường dẫn nơi lưu file.</param>
        public static void SaveBase64ToFile(this string base64String, string filePath)
        {
            if (string.IsNullOrWhiteSpace(base64String))
            {
                throw new ArgumentException("Chuỗi Base64 không hợp lệ.", nameof(base64String));
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Đường dẫn file không hợp lệ.", nameof(filePath));
            }

            try
            {
                // Chuyển chuỗi Base64 thành mảng byte
                byte[] fileBytes = Convert.FromBase64String(base64String);

                // Lưu mảng byte vào file
                File.WriteAllBytes(filePath, fileBytes);
            }
            catch (System.FormatException)
            {
                throw new ArgumentException("Chuỗi Base64 không đúng định dạng.", nameof(base64String));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Có lỗi xảy ra khi lưu file {filePath} từ chuỗi Base64.", ex);
            }
        }

        // Phương thức mở rộng để tạo mã vạch từ chuỗi
        public static byte[] GenerateBarcode(this string text, int width = 300, int height = 100)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Height = height,
                    Width = width,
                    Margin = 0 // Có thể thay đổi margin tùy ý
                }
            };

            var pixelData = writer.Write(text);
            using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                try
                {
                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }

                // Lưu ảnh vào một đường dẫn cố định để test
                string defaultPath = @"C:\Users\hung\Desktop\HIS\test print\barcode.png";
                Directory.CreateDirectory(Path.GetDirectoryName(defaultPath)); // Đảm bảo thư mục tồn tại
                bitmap.Save(defaultPath, System.Drawing.Imaging.ImageFormat.Png);

                using (var ms = new MemoryStream())
                {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// Chuyển text từ null về empty
        /// </summary>
        /// <param name="input">Đầu vào 1 text</param>
        /// <returns>1 string không bao giờ null</returns>
        public static string ToEmptyWhenNull([NotNullWhen(false)] this string? input)
        {
            return string.IsNullOrEmpty(input) ? string.Empty : input!;
        }

        /// <summary>
        /// Chuyển chữ tiếng việt sang chữ không dấu
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveDiacritics([NotNullWhen(false)] this string? text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            text = RemoveInvalidFileNameChars(text);
            // Xử lý các trường hợp đặc biệt
            text = text.Replace("đ", "d").Replace("Đ", "D");

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        // <summary>
        /// Tự động xóa các kí tự đặc biệt khi đặt tên file không được
        /// </summary>
        /// <param name="fileName">Đoạn mã cần chuyển đổi</param>
        public static string RemoveInvalidFileNameChars(string fileName)
        {
            // Lấy danh sách ký tự không hợp lệ trong tên file
            char[] invalidChars = Path.GetInvalidFileNameChars();

            // Loại bỏ các ký tự không hợp lệ khỏi tên file
            string cleanedFileName = string.Join("", fileName.Split(invalidChars));

            return cleanedFileName;
        }

        /// <summary>
        /// Thay thế các chuỗi trong văn bản dựa trên biểu thức chính quy (Regex).
        /// </summary>
        /// <param name="text">Chuỗi đầu vào cần xử lý.</param>
        /// <param name="paramRegex">Biểu thức chính quy (Regex) dùng để tìm kiếm.</param>
        /// <param name="replacement">Chuỗi thay thế cho các phần khớp với biểu thức chính quy.</param>
        /// <returns>Chuỗi đã thay thế hoặc chuỗi rỗng nếu đầu vào không hợp lệ.</returns>
        /// <remarks>
        /// - Nếu chuỗi đầu vào (`text`) là null hoặc rỗng, trả về chuỗi rỗng.
        /// - Sử dụng `Regex.Replace` để thay thế các phần tử phù hợp với biểu thức chính quy.
        /// </remarks>
        public static string ReplaceRegex([NotNullWhen(false)] this string? text, string paramRegex, string replacement)
        {
            // Kiểm tra chuỗi đầu vào có null hoặc rỗng không
            if (string.IsNullOrEmpty(text))
            {
                // Nếu null hoặc rỗng, trả về chuỗi rỗng
                return string.Empty;
            }

            // Thực hiện thay thế dựa trên biểu thức chính quy
            string result = Regex.Replace(text, paramRegex, replacement);

            // Trả về kết quả đã thay thế
            return result;
        }


        /// <summary>
        /// Đoàn Văn Tuân -> DOAN_VAN_TUAN
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ConvertCustomerNameToFileName(this string text)
        {
            var noDiacritics = text.RemoveDiacritics();
            string upperCase = noDiacritics.ToUpper();
            string result = Regex.Replace(upperCase, @"\s+", "_");

            return result;
        }

        public static string ConvertCustomerNameToNameSlug(this string text)
        {
            // Bỏ dấu tiếng Việt
            var noDiacritics = text.RemoveDiacritics();

            // Chuyển thành chữ thường
            string lowerCase = noDiacritics.ToLower();

            // Thay thế khoảng trắng (và tab) bằng dấu gạch ngang (-)
            string result = Regex.Replace(lowerCase, @"\s+", "-");

            return result;
        }


        /// <summary>
        /// xóa kí tự - ở đầu chuỗi
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveDashAtStart(this string text)
        {
            // Biểu thức chính quy để xóa dấu gạch ngang và khoảng trắng ở đầu mỗi dòng
            return Regex.Replace(text, @"^\s*-\s*", "");
        }

        /// <summary>
        /// Viết hoa mỗi đầu câu, ví dụ:"chữ in Hoa" => "Chữ In Hoa"
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToCapEachWord([NotNullWhen(false)] this string? input)
        {
            return string.IsNullOrEmpty(input) ? string.Empty : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower())!;
        }

        public static string ListToStringWithSemicolon(this List<string?>? input)
        {
            var rs = "";
            if (input == null || !input.Any())
            {
                return "";
            }

            foreach (var item in input)
            {
                if (item != null) rs += (item + ";");
            }
            return rs.TrimEnd(';');
        }

        public static string SelectAfterFirstSemicolon(this string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }

            var index = input.IndexOf(";");
            if (index == -1)
            {
                return input;  // Nếu không có dấu chấm phẩy, trả về chuỗi ban đầu
            }

            return input.Substring(index + 1);  // Trả về phần sau dấu chấm phẩy
        }

    }
}

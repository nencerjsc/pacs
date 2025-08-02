using DevExpress.Spreadsheet;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;

namespace NencerApi.Helpers
{
    public static class FileHelper
    {
        public static string ReadFileAsBase64(string filePath)
        {
            byte[] fileBytes = File.ReadAllBytes(filePath);
            return Convert.ToBase64String(fileBytes);
        }

        public static Stream Base64ToStream(string base64String)
        {
            byte[] byteArray = Convert.FromBase64String(base64String);
            return new MemoryStream(byteArray);
        }

        public static string StreamToBase64(Stream stream)
        {
            //if (stream == null)
            //    throw new ArgumentNullException(nameof(stream));

            //if (!stream.CanRead)
            //    throw new ArgumentException("Stream must be readable", nameof(stream));

            //if (!stream.CanSeek)
            //    throw new ArgumentException("Stream must be seekable", nameof(stream));

            stream.Position = 0;  // Đặt lại vị trí của stream về đầu
            byte[] byteArray = new byte[stream.Length];
            stream.Read(byteArray, 0, byteArray.Length);
            return Convert.ToBase64String(byteArray);
        }

        public static string GetFileBase64FromPath(string filePath)
        {
            try
            {
                // Đọc toàn bộ nội dung file dưới dạng byte array
                byte[] fileBytes = File.ReadAllBytes(filePath);

                // Chuyển đổi byte array thành chuỗi Base64
                string base64String = Convert.ToBase64String(fileBytes);

                return base64String;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

        public static DataTable ReadExcelFileToDataTable(Stream fileStream, int startRow = 1)
        {
            if (startRow < 1)
            {
                startRow = 1;
            }
            DataTable dataTable = new DataTable();

            using (Workbook workbook = new Workbook())
            {
                workbook.LoadDocument(fileStream);
                Worksheet worksheet = workbook.Worksheets[0];

                // Đọc tiêu đề (header)
                for (int col = 0; col <= worksheet.GetUsedRange().RightColumnIndex; col++)
                {
                    string columnName = worksheet.Cells[0, col].DisplayText; // Lấy nguyên text hiển thị
                    dataTable.Columns.Add(string.IsNullOrEmpty(columnName) ? $"Column{col + 1}" : columnName);
                }

                // Kiểm tra và thêm cột "GHI_CHU_LOI" nếu chưa có
                if (!dataTable.Columns.Contains("GHI_CHU_LOI"))
                {
                    dataTable.Columns.Add("GHI_CHU_LOI", typeof(string));
                }

                // Đọc dữ liệu từ Excel
                for (int row = startRow; row <= worksheet.GetUsedRange().BottomRowIndex; row++)
                {
                    DataRow dataRow = dataTable.NewRow();
                    for (int col = 0; col <= worksheet.GetUsedRange().RightColumnIndex; col++)
                    {
                        dataRow[col] = worksheet.Cells[row, col].DisplayText; // Lấy nguyên text từ cell
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }

            // Xóa các hàng trống khỏi DataTable
            var rowsToDelete = dataTable.AsEnumerable().Where(row =>
                row.ItemArray.All(field => string.IsNullOrEmpty(field?.ToString()))
            ).ToList();

            foreach (var row in rowsToDelete)
            {
                dataTable.Rows.Remove(row);
            }

            return dataTable;
        }

        public static string GenerateSlug(string text)
        {
            if (string.IsNullOrEmpty(text)) return "file";

            // Chuyển đổi tiếng Việt có dấu sang ASCII
            text = ConvertVietnameseToAscii(text);

            // Xóa ký tự đặc biệt, chỉ giữ lại chữ cái, số và dấu gạch ngang
            text = Regex.Replace(text, @"[^a-zA-Z0-9\s-]", "");

            // Thay thế khoảng trắng thành dấu gạch ngang
            text = Regex.Replace(text, @"\s+", "-").Trim();

            // Chuyển thành chữ thường
            return text.ToLower();
        }

        public static string ConvertVietnameseToAscii(string text)
        {
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }


    }
}

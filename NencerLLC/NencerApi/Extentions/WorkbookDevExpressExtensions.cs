using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Export;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace NencerApi.Extentions
{
    public static class WorkbookDevExpressExtensions
    {
        /// <summary>
        /// Lấy danh sách tên các sheet từ file Excel được truyền dưới dạng Base64.
        /// </summary>
        /// <param name="workbook">Đối tượng Workbook của DevExpress dùng để xử lý file Excel.</param>
        /// <param name="base64FileContent">Nội dung file Excel dưới dạng Base64.</param>
        /// <returns>Danh sách tên các sheet trong file Excel.</returns>
        /// <exception cref="ArgumentException">Ném lỗi nếu Base64 content bị null hoặc rỗng.</exception>
        /// <exception cref="InvalidOperationException">Ném lỗi nếu file Excel không hợp lệ.</exception>
        public static List<string> GetSheetNamesFromBase64(this Workbook workbook, string base64FileContent)
        {
            // Kiểm tra xem nội dung Base64 có hợp lệ hay không
            if (string.IsNullOrEmpty(base64FileContent))
            {
                throw new ArgumentException("Base64 content cannot be null or empty.");
            }

            // Chuyển đổi nội dung Base64 thành mảng byte
            byte[] fileBytes;
            try
            {
                fileBytes = Convert.FromBase64String(base64FileContent);
            }
            catch (FormatException)
            {
                throw new ArgumentException("Invalid Base64 content provided.");
            }

            // Tạo một MemoryStream để load dữ liệu từ mảng byte
            using (MemoryStream stream = new MemoryStream(fileBytes))
            {
                try
                {
                    // Load file Excel từ MemoryStream. DevExpress sẽ tự động nhận diện định dạng (XLS, XLSX).
                    workbook.LoadDocument(stream);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to load the Excel document. Ensure the Base64 content represents a valid Excel file.", ex);
                }
            }

            // Kiểm tra nếu không có sheet nào
            if (workbook.Worksheets.Count == 0)
            {
                return new List<string>(); // Trả về danh sách rỗng nếu không có sheet nào
            }

            // Tạo danh sách để lưu tên các sheet
            List<string> sheetNames = new List<string>();

            // Duyệt qua tất cả các sheet trong Workbook và lấy tên của chúng
            foreach (Worksheet sheet in workbook.Worksheets)
            {
                sheetNames.Add(sheet.Name); // Thêm tên của từng sheet vào danh sách
            }

            // Trả về danh sách tên các sheet
            return sheetNames;
        }

        public static string ConvertBase64ExcelToHtml(string base64Excel)
        {
            // Bước 1: Giải mã Base64 thành file Excel
            byte[] excelBytes = Convert.FromBase64String(base64Excel);
            string tempExcelPath = Path.Combine(Path.GetTempPath(), "tempExcel.xlsx");
            File.WriteAllBytes(tempExcelPath, excelBytes);

            // Bước 2: Chuyển đổi file Excel thành HTML
            Workbook workbook = new Workbook();
            workbook.LoadDocument(tempExcelPath);

            // Tạo một MemoryStream để lưu HTML
            using (MemoryStream htmlStream = new MemoryStream())
            {
                HtmlDocumentExporterOptions options = new HtmlDocumentExporterOptions
                {
                    EmbedImages = true // Nhúng hình ảnh vào HTML nếu có
                };

                workbook.ExportToHtml(htmlStream, options);

                // Chuyển đổi MemoryStream thành chuỗi HTML
                htmlStream.Position = 0;
                using (StreamReader reader = new StreamReader(htmlStream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static DataTable ReadExcelFileToDataTable(string filePath, int startRow = 1)
        {
            if (startRow < 1)
            {
                startRow = 1;
            }
            DataTable dataTable = new DataTable();

            using (Workbook workbook = new Workbook())
            {
                workbook.LoadDocument(filePath);
                Worksheet worksheet = workbook.Worksheets[0]; // Lấy sheet đầu tiên

                // Đọc tiêu đề (header)
                for (int col = 0; col <= worksheet.GetUsedRange().RightColumnIndex; col++)
                {
                    string columnName = worksheet.Cells[0, col].Value.TextValue;
                    dataTable.Columns.Add(string.IsNullOrEmpty(columnName) ? $"Column{col + 1}" : columnName);
                }

                // Kiểm tra và thêm cột "GHI_CHU_LOI" nếu chưa có
                if (!dataTable.Columns.Contains("GHI_CHU_LOI"))
                {
                    dataTable.Columns.Add("GHI_CHU_LOI", typeof(string));
                }

                // Đọc dữ liệu từ Excel
                for (int row = 1; row <= worksheet.GetUsedRange().BottomRowIndex; row++)
                {
                    DataRow dataRow = dataTable.NewRow();
                    for (int col = 0; col <= worksheet.GetUsedRange().RightColumnIndex; col++)
                    {
                        dataRow[col] = worksheet.Cells[row, col].Value.TextValue;
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }

            // Lấy danh sách các hàng trống cần xóa
            var rowsToDelete = dataTable.AsEnumerable().Where(row =>
                row.ItemArray.All(field => field == null || field == DBNull.Value || string.IsNullOrEmpty(field.ToString()))
            ).ToList();

            // Xóa các hàng dữ liệu trống khỏi DataTable
            foreach (var row in rowsToDelete)
            {
                dataTable.Rows.Remove(row);
            }

            return dataTable;
        }

        public static DataTable ReadExcelFileToDataTable(Stream fileStream, int startRow = 1, int? endRow = null, bool? addedErrorCoumn = true)
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

                int rightCol = worksheet.GetUsedRange().RightColumnIndex;
                int bottomRow = worksheet.GetUsedRange().BottomRowIndex;

                // Nếu endRow được truyền vào và nhỏ hơn dòng cuối, thì dùng endRow
                int lastRow = endRow.HasValue && endRow.Value <= bottomRow ? endRow.Value : bottomRow;

                // Đọc tiêu đề (header)
                for (int col = 0; col <= rightCol; col++)
                {
                    string columnName = worksheet.Cells[0, col].DisplayText;
                    dataTable.Columns.Add(string.IsNullOrEmpty(columnName) ? $"Column{col + 1}" : columnName);
                }

                // Thêm cột ghi chú lỗi nếu cần
                if (!dataTable.Columns.Contains("GHI_CHU_LOI") && addedErrorCoumn == true)
                {
                    dataTable.Columns.Add("GHI_CHU_LOI", typeof(string));
                }

                // Đọc dữ liệu từ startRow đến endRow
                for (int row = startRow; row <= lastRow; row++)
                {
                    DataRow dataRow = dataTable.NewRow();
                    for (int col = 0; col <= rightCol; col++)
                    {
                        dataRow[col] = worksheet.Cells[row, col].DisplayText;
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }

            // Xóa các hàng trống
            var rowsToDelete = dataTable.AsEnumerable().Where(row =>
                row.ItemArray.All(field => string.IsNullOrEmpty(field?.ToString()))
            ).ToList();

            foreach (var row in rowsToDelete)
            {
                dataTable.Rows.Remove(row);
            }

            return dataTable;
        }



        public static string GenerateExcelTemplateAsBase64(this Workbook workbook, List<string> columnNames)
        {
            Worksheet worksheet = workbook.Worksheets[0];

            // Ghi tiêu đề cột vào hàng đầu tiên
            for (int colIndex = 0; colIndex < columnNames.Count; colIndex++)
            {
                worksheet.Cells[0, colIndex].Value = columnNames[colIndex];
            }

            // Định dạng tiêu đề (in đậm, căn giữa, border)
            CellRange headerRange = worksheet.Range.FromLTRB(0, 0, columnNames.Count - 1, 0);
            headerRange.Font.Bold = true;
            headerRange.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
            headerRange.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
            headerRange.Borders.SetAllBorders(System.Drawing.Color.Black, BorderLineStyle.Thin);

            // Auto-fit cột
            worksheet.Columns.AutoFit(0, columnNames.Count - 1);

            // Lưu file vào MemoryStream
            using (MemoryStream stream = new MemoryStream())
            {
                workbook.SaveDocument(stream, DevExpress.Spreadsheet.DocumentFormat.Xlsx);
                return Convert.ToBase64String(stream.ToArray()); // Chuyển thành base64
            }
        }

        public static MemoryStream GenerateExcelTemplateAsFile(this Workbook workbook, List<string> columnNames)
        {
            Worksheet worksheet = workbook.Worksheets[0];

            // Ghi tiêu đề cột vào hàng đầu tiên
            for (int colIndex = 0; colIndex < columnNames.Count; colIndex++)
            {
                worksheet.Cells[0, colIndex].Value = columnNames[colIndex];
            }

            // Định dạng tiêu đề (in đậm, căn giữa, border)
            CellRange headerRange = worksheet.Range.FromLTRB(0, 0, columnNames.Count - 1, 0);
            headerRange.Font.Bold = true;
            headerRange.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
            headerRange.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
            headerRange.Borders.SetAllBorders(System.Drawing.Color.Black, BorderLineStyle.Thin);

            // Auto-fit cột
            worksheet.Columns.AutoFit(0, columnNames.Count - 1);

            // Lưu file vào MemoryStream
            MemoryStream stream = new MemoryStream();
            workbook.SaveDocument(stream, DevExpress.Spreadsheet.DocumentFormat.Xlsx);
            stream.Position = 0; // Đảm bảo đọc từ đầu file

            return stream; // Trả về MemoryStream chứa file
        }

        public static MemoryStream GenerateExcelFromDataTable(this Workbook workbook, DataTable dataTable)
        {
            Worksheet worksheet = workbook.Worksheets[0];

            // Xóa dữ liệu cũ (nếu có)
            worksheet.Cells.ClearContents();

            // Lấy số lượng cột & tên cột từ DataTable
            int columnCount = dataTable.Columns.Count;
            int rowCount = dataTable.Rows.Count;

            // 1️⃣ Tạo tiêu đề cột trong Excel
            for (int col = 0; col < columnCount; col++)
            {
                worksheet.Cells[0, col].Value = dataTable.Columns[col].ColumnName;
            }

            // 3️⃣ Điền dữ liệu từ DataTable vào Excel
            for (int row = 0; row < rowCount; row++)
            {
                for (int col = 0; col < columnCount; col++)
                {
                    object value = dataTable.Rows[row][col];
                    if (value != DBNull.Value)
                    {
                        worksheet.Cells[row + 1, col].Value = value.ToString();
                    }
                }
            }

            // 4️⃣ Auto-fit cột
            worksheet.Columns.AutoFit(0, columnCount - 1);

            // 5️⃣ Xuất file Excel ra MemoryStream
            MemoryStream stream = new MemoryStream();
            workbook.SaveDocument(stream, DevExpress.Spreadsheet.DocumentFormat.Xlsx);
            stream.Position = 0; // Đảm bảo đọc từ đầu file

            return stream; // Trả về MemoryStream chứa file
        }
    }
}

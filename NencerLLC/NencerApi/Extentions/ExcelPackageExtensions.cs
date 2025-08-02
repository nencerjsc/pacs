using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Data;
using System.Drawing;
using System.Collections.Generic;

namespace NencerApi.Extensions
{
    public static class ExcelPackageExtensions
    {
        /// <summary>
        /// Chuyển đổi ExcelPackage thành chuỗi base64.
        /// </summary>
        /// <param name="package">Đối tượng ExcelPackage để chuyển đổi.</param>
        /// <returns>Chuỗi base64 đại diện cho nội dung của ExcelPackage.</returns>
        public static string ToBase64(this ExcelPackage package)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Lưu ExcelPackage vào MemoryStream
                package.SaveAs(memoryStream);

                // Đảm bảo con trỏ luồng ở đầu luồng để đọc từ đầu
                memoryStream.Position = 0;

                // Đọc tất cả dữ liệu từ MemoryStream và chuyển đổi thành base64
                byte[] bytes = memoryStream.ToArray();
                return Convert.ToBase64String(bytes);
            }
        }

        public static void ReplaceText(this ExcelPackage package, int sheetIndex, Dictionary<string, string> replacements)
        {
            if (sheetIndex < 0 || sheetIndex >= package.Workbook.Worksheets.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(sheetIndex), "Sheet index is out of range.");
            }

            var worksheet = package.Workbook.Worksheets[sheetIndex];

            for (int row = worksheet.Dimension.Start.Row; row <= worksheet.Dimension.End.Row; row++)
            {
                for (int col = worksheet.Dimension.Start.Column; col <= worksheet.Dimension.End.Column; col++)
                {
                    var cell = worksheet.Cells[row, col];
                    if (cell.Value != null)
                    {
                        string cellText = cell.Value.ToString();
                        foreach (var kvp in replacements)
                        {
                            if (cellText.Contains(kvp.Key))
                            {
                                cellText = cellText.Replace(kvp.Key, kvp.Value);
                            }
                        }
                        cell.Value = cellText;
                    }
                }
            }
        }

        // Replace data từ Dictionary vào một sheet
        public static void ReplaceDataFromDictionary(this ExcelPackage package, int sheetIndex, Dictionary<string, string> replacements)
        {
            if (sheetIndex < 0 || sheetIndex >= package.Workbook.Worksheets.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(sheetIndex), "Sheet index is out of range.");
            }

            var worksheet = package.Workbook.Worksheets[sheetIndex];

            for (int row = worksheet.Dimension.Start.Row; row <= worksheet.Dimension.End.Row; row++)
            {
                for (int col = worksheet.Dimension.Start.Column; col <= worksheet.Dimension.End.Column; col++)
                {
                    var cell = worksheet.Cells[row, col];
                    if (cell.Value != null)
                    {
                        string cellText = cell.Value.ToString();
                        foreach (var kvp in replacements)
                        {
                            if (cellText.Contains(kvp.Key))
                            {
                                cellText = cellText.Replace(kvp.Key, kvp.Value);
                            }
                        }
                        cell.Value = cellText;
                    }
                }
            }
        }

        // Phương pháp 1: Sử dụng LoadFromDataTable để chèn toàn bộ bảng
        public static void InsertDataTable_ByAllTable(this ExcelPackage package, int sheetIndex, string startCell, DataTable dataTable, bool hasSumCol = false, List<int> listSumColIndex = null)
        {
            if (sheetIndex < 0 || sheetIndex >= package.Workbook.Worksheets.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(sheetIndex), "Sheet index is out of range.");
            }

            var worksheet = package.Workbook.Worksheets[sheetIndex];
            int startRow = worksheet.Cells[startCell].Start.Row;
            int startCol = worksheet.Cells[startCell].Start.Column;

            worksheet.InsertRow(startRow + 1, dataTable.Rows.Count);

            for (int col = 0; col < dataTable.Columns.Count; col++)
            {
                worksheet.Cells[startRow, startCol + col].Value = dataTable.Columns[col].ColumnName;
                worksheet.Cells[startRow, startCol + col].Style.Font.Bold = true;
                worksheet.Cells[startRow, startCol + col].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            }

            var dataRange = worksheet.Cells[startRow + 1, startCol].LoadFromDataTable(dataTable, false);

            // Kẻ viền cho toàn bộ dải ô chứa dữ liệu
            var fullDataRange = worksheet.Cells[startRow + 1, startCol, startRow + dataTable.Rows.Count, startCol + dataTable.Columns.Count - 1];
            fullDataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            fullDataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            fullDataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            fullDataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            fullDataRange.Style.Border.Top.Color.SetColor(Color.Black);
            fullDataRange.Style.Border.Bottom.Color.SetColor(Color.Black);
            fullDataRange.Style.Border.Left.Color.SetColor(Color.Black);
            fullDataRange.Style.Border.Right.Color.SetColor(Color.Black);

            int totalRowIndex = startRow + dataTable.Rows.Count + 1;
            if (hasSumCol == true && listSumColIndex != null && listSumColIndex.Count > 0)
            {
                int firstSumColIndex = listSumColIndex[0];
                int targetCol = startCol + firstSumColIndex;

                if (firstSumColIndex > 0)
                {
                    var mergeRange = worksheet.Cells[totalRowIndex, startCol, totalRowIndex, targetCol - 1];
                    mergeRange.Merge = true;
                    mergeRange.Value = "Tổng số:";
                    mergeRange.Style.Font.Bold = true;
                    mergeRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                }

                foreach (int columnIndex in listSumColIndex)
                {
                    int sumCol = startCol + columnIndex;
                    worksheet.Cells[totalRowIndex, sumCol].Formula = $"SUM({worksheet.Cells[startRow + 1, sumCol].Address}:{worksheet.Cells[totalRowIndex - 1, sumCol].Address})";
                    worksheet.Cells[totalRowIndex, sumCol].Style.Font.Bold = true;
                }

                var totalRowRange = worksheet.Cells[totalRowIndex, startCol, totalRowIndex, startCol + dataTable.Columns.Count - 1];
                totalRowRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                totalRowRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                totalRowRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                totalRowRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                totalRowRange.Style.Border.Top.Color.SetColor(Color.Black);
                totalRowRange.Style.Border.Bottom.Color.SetColor(Color.Black);
                totalRowRange.Style.Border.Left.Color.SetColor(Color.Black);
                totalRowRange.Style.Border.Right.Color.SetColor(Color.Black);
            }
        }

        // Phương pháp 2: Sử dụng foreach để chèn từng dòng
        public static void InsertDataTable_ByEachRow(this ExcelPackage package, int sheetIndex, string startCell, DataTable dataTable, bool hasSumCol = false, List<int>? listSumColIndex = null)
        {
            if (sheetIndex < 0 || sheetIndex >= package.Workbook.Worksheets.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(sheetIndex), "Sheet index is out of range.");
            }

            var worksheet = package.Workbook.Worksheets[sheetIndex];
            int startRow = worksheet.Cells[startCell].Start.Row;
            int startCol = worksheet.Cells[startCell].Start.Column;

            worksheet.InsertRow(startRow + 1, dataTable.Rows.Count);

            for (int col = 0; col < dataTable.Columns.Count; col++)
            {
                worksheet.Cells[startRow, startCol + col].Value = dataTable.Columns[col].ColumnName;
                worksheet.Cells[startRow, startCol + col].Style.Font.Bold = true;
                worksheet.Cells[startRow, startCol + col].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            }

            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                for (int col = 0; col < dataTable.Columns.Count; col++)
                {
                    worksheet.Cells[startRow + row + 1, startCol + col].Value = dataTable.Rows[row][col];
                    worksheet.Cells[startRow + row + 1, startCol + col].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                }
            }

            int totalRowIndex = startRow + dataTable.Rows.Count + 1;
            if (hasSumCol == true && listSumColIndex != null && listSumColIndex.Count > 0)
            {
                int firstSumColIndex = listSumColIndex[0];
                int targetCol = startCol + firstSumColIndex;

                if (firstSumColIndex > 0)
                {
                    var mergeRange = worksheet.Cells[totalRowIndex, startCol, totalRowIndex, targetCol - 1];
                    mergeRange.Merge = true;
                    mergeRange.Value = "Tổng số:";
                    mergeRange.Style.Font.Bold = true;
                    mergeRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                }

                foreach (int columnIndex in listSumColIndex)
                {
                    int sumCol = startCol + columnIndex;
                    worksheet.Cells[totalRowIndex, sumCol].Formula = $"SUM({worksheet.Cells[startRow + 1, sumCol].Address}:{worksheet.Cells[totalRowIndex - 1, sumCol].Address})";
                    worksheet.Cells[totalRowIndex, sumCol].Style.Font.Bold = true;
                }

                var totalRowRange = worksheet.Cells[totalRowIndex, startCol, totalRowIndex, startCol + dataTable.Columns.Count - 1];
                totalRowRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                totalRowRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                totalRowRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                totalRowRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                totalRowRange.Style.Border.Top.Color.SetColor(Color.Black);
                totalRowRange.Style.Border.Bottom.Color.SetColor(Color.Black);
                totalRowRange.Style.Border.Left.Color.SetColor(Color.Black);
                totalRowRange.Style.Border.Right.Color.SetColor(Color.Black);
            }
        }
    }
}

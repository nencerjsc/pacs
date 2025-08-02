using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Data;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using DataTable = System.Data.DataTable;
using OfficeOpenXml.FormulaParsing.Excel;

namespace NencerApi.Extensions
{
    public static class ExcelWorksheetExtensions
    {
        // Replace data từ Dictionary vào một sheet
        public static void ReplaceDataFromDictionary(this ExcelWorksheet worksheet, Dictionary<string, string> replacements)
        {
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

        /// <summary>
        /// Thay thế dữ liệu trong ExcelWorksheet dựa trên cặp key-value từ một DataTable chỉ có một dòng.
        /// </summary>
        /// <param name="worksheet">Worksheet để thay thế dữ liệu.</param>
        /// <param name="dataTable">DataTable chứa cặp key-value để thay thế, với key là tên cột và value là giá trị tương ứng.</param>
        public static void ReplaceDataFromSingleRowDataTable(this ExcelWorksheet worksheet, DataTable? dataTable)
        {
            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return;
            }

            if (worksheet.Dimension == null)
            {
                throw new InvalidOperationException("Worksheet không có dữ liệu để xử lý.");
            }

            var row = dataTable.Rows[0];

            for (int rowIndex = worksheet.Dimension.Start.Row; rowIndex <= worksheet.Dimension.End.Row; rowIndex++)
            {
                for (int colIndex = worksheet.Dimension.Start.Column; colIndex <= worksheet.Dimension.End.Column; colIndex++)
                {
                    var cell = worksheet.Cells[rowIndex, colIndex];
                    if (cell.Value != null)
                    {
                        string cellText = cell.Value.ToString();
                        foreach (DataColumn column in dataTable.Columns)
                        {
                            string key = column.ColumnName;
                            string value = row[column].ToString();
                            if (cellText.Contains(key))
                            {
                                cellText = cellText.Replace(key, value);
                            }
                        }
                        cell.Value = cellText;
                    }
                }
            }
        }

        /// <summary>
        /// Thay thế dữ liệu trong ExcelWorksheet dựa trên một cặp key-value.
        /// </summary>
        /// <param name="worksheet">Worksheet cần thực hiện thay thế.</param>
        /// <param name="key">Key cần tìm trong worksheet.</param>
        /// <param name="value">Giá trị thay thế cho key.</param>
        public static void ReplaceDataByKeyValue(this ExcelWorksheet worksheet, string key, string value)
        {
            if (worksheet.Dimension == null)
            {
                throw new InvalidOperationException("Worksheet không có dữ liệu để xử lý.");
            }

            for (int rowIndex = worksheet.Dimension.Start.Row; rowIndex <= worksheet.Dimension.End.Row; rowIndex++)
            {
                for (int colIndex = worksheet.Dimension.Start.Column; colIndex <= worksheet.Dimension.End.Column; colIndex++)
                {
                    var cell = worksheet.Cells[rowIndex, colIndex];
                    if (cell.Value != null)
                    {
                        string cellText = cell.Value.ToString();
                        if (cellText.Contains(key))
                        {
                            cell.Value = cellText.Replace(key, value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Thay thế dữ liệu trong ExcelWorksheet dựa trên một DataTable chứa các cặp key-value.
        /// </summary>
        /// <param name="worksheet">Worksheet cần thực hiện thay thế.</param>
        /// <param name="dataTable">DataTable chứa các cặp key-value để thay thế.</param>
        public static void ReplaceDataFromKeyValueTable(this ExcelWorksheet worksheet, DataTable dataTable)
        {
            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return;
            }

            //// Kiểm tra cấu trúc DataTable phải có 2 cột
            //if (dataTable.Columns.Count != 2)
            //{
            //    throw new ArgumentException("DataTable phải chứa đúng 2 cột: Key và Value.");
            //}

            foreach (DataRow row in dataTable.Rows)
            {
                string key = row[0].ToString();
                string value = row[1].ToString();

                if (!string.IsNullOrEmpty(key) && value != null)
                {
                    worksheet.ReplaceDataByKeyValue(key, value);
                }
            }
        }



        // Phương pháp 1: Sử dụng LoadFromDataTable để chèn toàn bộ bảng vào worksheet
        // Tốc độ nhanh hơn đối với dữ liệu lớn
        public static void InsertDataTable_ByAllTable(this ExcelWorksheet worksheet, string startHeaderCell, DataTable? dataTable, bool hasSumCol = false, List<int> listSumColIndex = null, bool? hasHiddenCol = false, bool? isCustomCellStyleByValue = false)
        {
            if (dataTable == null)
            {
                return;
            }

            int startRow = worksheet.Cells[startHeaderCell].Start.Row;
            int startCol = worksheet.Cells[startHeaderCell].Start.Column;

            // Chèn tiêu đề cột từ DataTable (luôn luôn chèn dù không có dòng dữ liệu)
            for (int col = 0; col < dataTable.Columns.Count; col++)
            {
                worksheet.Cells[startRow, startCol + col].Value = dataTable.Columns[col].ColumnName;
                worksheet.Cells[startRow, startCol + col].Style.Font.Bold = true;
                worksheet.Cells[startRow, startCol + col].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            }

            // Nếu DataTable có dữ liệu, thực hiện chèn nội dung
            if (dataTable.Rows.Count > 0)
            {
                worksheet.InsertRow(startRow + 1, dataTable.Rows.Count);
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
                if (hasSumCol && listSumColIndex != null && listSumColIndex.Count > 0)
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

                if (hasHiddenCol == true)
                {
                    // Ẩn các cột có tên bắt đầu bằng "hidden_"
                    for (int col = 0; col < dataTable.Columns.Count; col++)
                    {
                        if (dataTable.Columns[col].ColumnName.StartsWith("hidden_"))
                        {
                            worksheet.Column(startCol + col).Hidden = true; // Ẩn cột
                        }
                    }
                }

                if (isCustomCellStyleByValue == true)
                {
                    for (int col = 0; col < dataTable.Columns.Count; col++)
                    {
                        if (dataTable.Columns[col].ColumnName.StartsWith("hidden_"))
                        {
                            int hiddenColIndex = startCol + col;

                            // Xác định cột ngay cạnh bên trái
                            int leftColIndex = hiddenColIndex - 1;
                            if (leftColIndex < startCol)
                            {
                                continue; // Nếu không có cột bên trái (cột đầu tiên), bỏ qua
                            }

                            // Duyệt qua tất cả các hàng của cột hidden_
                            for (int row = 0; row < dataTable.Rows.Count; row++)
                            {
                                // Lấy giá trị trong cột hidden_
                                var compareValue = worksheet.Cells[startRow + 1 + row, hiddenColIndex].Value;

                                if (compareValue != null && decimal.TryParse(compareValue.ToString(), out decimal compareDecimal))
                                {
                                    var cell = worksheet.Cells[startRow + 1 + row, leftColIndex];

                                    // Định dạng dựa trên giá trị compareDecimal
                                    if (compareDecimal > 0)
                                    {
                                        cell.Style.Font.Color.SetColor(Color.Red);
                                        cell.Style.Font.Bold = true;
                                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right; // Căn phải
                                    }
                                    else if (compareDecimal < 0)
                                    {
                                        cell.Style.Font.Color.SetColor(Color.Blue);
                                        cell.Style.Font.Bold = true;
                                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left; // Căn trái
                                    }
                                    else
                                    {
                                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Căn giữa nếu compare = 0
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }



        public static void GroupByColumnName(
            this ExcelWorksheet worksheet,
            string startCell,
            DataTable dataTable,
            string groupByColumnName,
            bool? reIndexByGroup,
            int mergeUntilColumnIndex)
        {
            // Lấy tọa độ bắt đầu từ startCell
            int dataStartRow = worksheet.Cells[startCell].Start.Row;
            int dataStartCol = worksheet.Cells[startCell].Start.Column;
            int totalRows = dataTable.Rows.Count;
            int totalCols = dataTable.Columns.Count;

            // Xác định cột chứa giá trị cần group
            int groupByColIndex = -1;
            for (int col = 0; col < totalCols; col++)
            {
                var cellValue = worksheet.Cells[dataStartRow, dataStartCol + col].Text?.Trim(); // Lấy giá trị hiển thị
                if (cellValue == groupByColumnName)
                {
                    groupByColIndex = dataStartCol + col;
                    break;
                }
            }

            if (groupByColIndex == -1)
            {
                throw new ArgumentException($"Không tìm thấy cột có tiêu đề '{groupByColumnName}' trong tiêu đề.");
            }

            // Lấy giá trị duy nhất từ cột group
            var uniqueGroups = new HashSet<string>();
            for (int row = dataStartRow + 1; row <= dataStartRow + totalRows; row++) // +1 để bỏ qua tiêu đề
            {
                var cellValue = worksheet.Cells[row, groupByColIndex].Text?.Trim(); // Lấy giá trị hiển thị
                if (!string.IsNullOrEmpty(cellValue))
                {
                    uniqueGroups.Add(cellValue);
                }
            }

            // Duyệt qua từng nhóm và thêm dòng mới
            int currentRowOffset = 0; // Dùng để điều chỉnh vị trí dòng khi thêm dòng mới
            int groupIndex = 1;
            foreach (var groupValue in uniqueGroups)
            {
                // Tìm tất cả các hàng thuộc nhóm hiện tại
                var groupRows = new List<int>();
                for (int row = dataStartRow + 1; row <= dataStartRow + totalRows; row++)
                {
                    var cellValue = worksheet.Cells[row + currentRowOffset, groupByColIndex].Text?.Trim(); // Lấy giá trị hiển thị
                    if (cellValue == groupValue)
                    {
                        groupRows.Add(row + currentRowOffset);
                    }
                }

                if (groupRows.Count > 0)
                {
                    // Thêm một dòng mới trước nhóm
                    int newRowIndex = groupRows[0];
                    worksheet.InsertRow(newRowIndex, 1); // Thêm dòng mới
                    currentRowOffset++; // Cập nhật offset sau khi thêm dòng

                    // Hợp nhất các ô từ cột thứ 2 đến cột mergeUntilColumnIndex
                    var mergeRange = worksheet.Cells[newRowIndex, dataStartCol + 1, newRowIndex, dataStartCol + mergeUntilColumnIndex - 1];
                    mergeRange.Merge = true;
                    mergeRange.Value = $"{groupValue}"; // Hiển thị "Tên nhóm"
                    mergeRange.Style.Font.Bold = true;
                    mergeRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left; // Căn lề trái
                    mergeRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    mergeRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    mergeRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(252, 228, 214)); // Màu nền cam nhạt
                    mergeRange.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                    // Đánh số thứ tự nhóm trong cột STT
                    worksheet.Cells[newRowIndex, dataStartCol].Value = groupIndex.ToString();
                    worksheet.Cells[newRowIndex, dataStartCol].Style.Font.Bold = true;
                    worksheet.Cells[newRowIndex, dataStartCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    // Áp dụng màu nền cho toàn bộ dòng nhóm
                    for (int col = dataStartCol; col < dataStartCol + totalCols; col++)
                    {
                        var cell = worksheet.Cells[newRowIndex, col];
                        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(252, 228, 214)); // Màu nền cam nhạt
                    }

                    // Tính tổng riêng cho từng cột sau mergeUntilColumnIndex
                    for (int col = mergeUntilColumnIndex; col < totalCols; col++)
                    {
                        double sum = 0;

                        // Tính tổng cho nhóm hiện tại
                        foreach (var row in groupRows)
                        {
                            var cellValue = worksheet.Cells[row, dataStartCol + col].Value;
                            if (cellValue != null && double.TryParse(cellValue.ToString(), out double cellNumber))
                            {
                                sum += cellNumber;
                            }
                        }

                        // Gán giá trị tổng vào ô tương ứng trong dòng nhóm
                        worksheet.Cells[newRowIndex, dataStartCol + col].Value = sum;
                        worksheet.Cells[newRowIndex, dataStartCol + col].Style.Font.Bold = true;
                        worksheet.Cells[newRowIndex, dataStartCol + col].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[newRowIndex, dataStartCol + col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    // Kiểm tra tùy chọn reIndexByGroup
                    if (reIndexByGroup == true)
                    {
                        // Đánh số thứ tự lại cho từng dòng trong nhóm
                        int itemIndex = 1;
                        foreach (var row in groupRows)
                        {
                            worksheet.Cells[row + 1, dataStartCol].Value = itemIndex++; // Đánh số thứ tự dòng trong nhóm
                        }
                    }

                    groupIndex++; // Tăng chỉ số nhóm
                }
            }

            // Ẩn cột chứa giá trị group
            worksheet.Column(groupByColIndex).Hidden = true;
        }

        #region chèn bảng
        public static void InsertAndGroupDataTable(
            this ExcelWorksheet worksheet,
            string startHeaderCell,
            DataTable? dataTable,
            string? groupByColumnName = null,
            bool reIndexByGroup = true,
            int mergeCellIndex = 2,
            bool? hasHiddenCol = false,
            bool? isCustomCellStyleByValue = false
        )
        {
            if (dataTable == null)
            {
                return;
            }

            bool hasSumCol = mergeCellIndex != 0;
            int startRow = worksheet.Cells[startHeaderCell].Start.Row;
            int startCol = worksheet.Cells[startHeaderCell].Start.Column;

            // Xác định các cột cần tính tổng và cập nhật tiêu đề cột
            var sumColumnIndices = new List<int>();
            for (int col = 0; col < dataTable.Columns.Count; col++)
            {
                string columnName = dataTable.Columns[col].ColumnName;

                // Kiểm tra đuôi _sum
                if (columnName.EndsWith("_sum"))
                {
                    sumColumnIndices.Add(col); // Thêm vào danh sách các cột cần tính tổng
                    columnName = columnName.Replace("_sum", ""); // Loại bỏ đuôi _sum
                }

                // Ghi tiêu đề vào Excel
                worksheet.Cells[startRow, startCol + col].Value = columnName;
                worksheet.Cells[startRow, startCol + col].Style.Font.Bold = true;
                worksheet.Cells[startRow, startCol + col].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            }

            // Sử dụng sumColumnIndices thay vì listSumColIndex
            var listSumColIndex = sumColumnIndices;

            // Chèn dữ liệu
            if (dataTable.Rows.Count > 0)
            {
                worksheet.InsertRow(startRow + 1, dataTable.Rows.Count);
                var dataRange = worksheet.Cells[startRow + 1, startCol].LoadFromDataTable(dataTable, false);

                // Kẻ viền cho toàn bộ dữ liệu
                var fullDataRange = worksheet.Cells[startRow + 1, startCol, startRow + dataTable.Rows.Count, startCol + dataTable.Columns.Count - 1];
                fullDataRange.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                // Kẻ viền cho từng dòng dữ liệu
                for (int row = startRow + 1; row <= startRow + dataTable.Rows.Count; row++)
                {
                    var rowRange = worksheet.Cells[row, startCol, row, startCol + dataTable.Columns.Count - 1];
                    rowRange.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                }

                // Không có nhóm: Sử dụng công thức Excel để tính tổng
                if (string.IsNullOrEmpty(groupByColumnName) && hasSumCol && listSumColIndex.Count > 0)
                {
                    int totalRowIndex = startRow + dataTable.Rows.Count + 1;
                    foreach (int columnIndex in listSumColIndex)
                    {
                        int sumCol = startCol + columnIndex;
                        worksheet.Cells[totalRowIndex, sumCol].Formula =
                            $"SUM({worksheet.Cells[startRow + 1, sumCol].Address}:{worksheet.Cells[totalRowIndex - 1, sumCol].Address})";
                        worksheet.Cells[totalRowIndex, sumCol].Style.Font.Bold = true;
                    }

                    var mergeRange = worksheet.Cells[totalRowIndex, startCol, totalRowIndex, startCol + mergeCellIndex - 1];
                    mergeRange.Merge = true;
                    mergeRange.Value = "Tổng số:";
                    mergeRange.Style.Font.Bold = true;
                    mergeRange.Style.Font.Color.SetColor(Color.DarkRed);
                    mergeRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    mergeRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    mergeRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(217, 225, 242));

                    // Định dạng toàn bộ dòng tổng
                    var totalRowRange = worksheet.Cells[totalRowIndex, startCol, totalRowIndex, startCol + dataTable.Columns.Count - 1];
                    totalRowRange.Style.Font.Color.SetColor(Color.DarkRed);
                    totalRowRange.Style.Font.Bold = true;
                    totalRowRange.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    totalRowRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    totalRowRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(217, 225, 242));
                }

                // Có nhóm: Tính tổng từng nhóm và tổng chung bỏ qua các dòng nhóm
                if (!string.IsNullOrEmpty(groupByColumnName))
                {
                    var uniqueGroups = new HashSet<string>();
                    int groupByColIndex = -1;

                    // Xác định cột chứa giá trị cần nhóm
                    for (int col = 0; col < dataTable.Columns.Count; col++)
                    {
                        if (dataTable.Columns[col].ColumnName == groupByColumnName)
                        {
                            groupByColIndex = startCol + col;
                            break;
                        }
                    }

                    if (groupByColIndex == -1)
                    {
                        throw new ArgumentException($"Không tìm thấy cột cần nhóm '{groupByColumnName}' trong DataTable.");
                    }

                    // Lấy danh sách giá trị nhóm
                    for (int row = startRow + 1; row <= startRow + dataTable.Rows.Count; row++)
                    {
                        var cellValue = worksheet.Cells[row, groupByColIndex].Text?.Trim();
                        if (!string.IsNullOrEmpty(cellValue))
                        {
                            uniqueGroups.Add(cellValue);
                        }
                    }

                    // Thêm các nhóm và tính tổng theo nhóm
                    int currentRowOffset = 0;
                    int groupIndex = 1;
                    foreach (var groupValue in uniqueGroups)
                    {
                        var groupRows = new List<int>();

                        // Xác định các hàng thuộc nhóm hiện tại
                        for (int row = startRow + 1; row <= startRow + dataTable.Rows.Count + currentRowOffset; row++)
                        {
                            var cellValue = worksheet.Cells[row, groupByColIndex].Text?.Trim();
                            if (cellValue == groupValue)
                            {
                                groupRows.Add(row);
                            }
                        }

                        if (groupRows.Count > 0)
                        {
                            int newRowIndex = groupRows.First();
                            worksheet.InsertRow(newRowIndex, 1);
                            currentRowOffset++;

                            // Merge và ghi giá trị nhóm
                            var mergeRange = worksheet.Cells[newRowIndex, startCol + 1, newRowIndex, startCol + mergeCellIndex - 1];
                            mergeRange.Merge = true;
                            mergeRange.Value = groupValue;
                            mergeRange.Style.Font.Bold = true;
                            mergeRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            mergeRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            mergeRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(252, 228, 214));
                            mergeRange.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                            // Tô màu toàn bộ dòng nhóm
                            var groupRowRange = worksheet.Cells[newRowIndex, startCol, newRowIndex, startCol + dataTable.Columns.Count - 1];
                            groupRowRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            groupRowRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(252, 228, 214));
                            groupRowRange.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                            // Đánh số thứ tự nhóm
                            worksheet.Cells[newRowIndex, startCol].Value = groupIndex++;
                            worksheet.Cells[newRowIndex, startCol].Style.Font.Bold = true;

                            // Tính tổng trong nhóm
                            foreach (var columnIndex in listSumColIndex)
                            {
                                int sumCol = startCol + columnIndex;
                                double sum = 0;

                                foreach (var row in groupRows)
                                {
                                    var cellValue = worksheet.Cells[row + 1, sumCol].Value;
                                    if (cellValue != null && double.TryParse(cellValue.ToString(), out double cellNumber))
                                    {
                                        sum += cellNumber;
                                    }

                                    // Kẻ viền cho từng dòng trong nhóm
                                    var itemRowRange = worksheet.Cells[row, startCol, row, startCol + dataTable.Columns.Count - 1];
                                    itemRowRange.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                }

                                worksheet.Cells[newRowIndex, sumCol].Value = sum;
                                worksheet.Cells[newRowIndex, sumCol].Style.Font.Bold = true;
                            }
                        }
                    }

                    // Ẩn cột nhóm
                    worksheet.Column(groupByColIndex).Hidden = true;

                    // Tính tổng cuối bảng (chỉ dòng không phải dòng nhóm)
                    if (hasSumCol && listSumColIndex.Count > 0)
                    {
                        int totalRowIndex = startRow + dataTable.Rows.Count + currentRowOffset + 1;
                        foreach (int columnIndex in listSumColIndex)
                        {
                            int sumCol = startCol + columnIndex;
                            double totalSum = 0;

                            for (int row = startRow + 1; row < totalRowIndex; row++)
                            {
                                if (worksheet.Cells[row, startCol + 1].Merge) // Dòng nhóm
                                    continue;

                                var cellValue = worksheet.Cells[row, sumCol].Value;
                                if (cellValue != null && double.TryParse(cellValue.ToString(), out double cellNumber))
                                {
                                    totalSum += cellNumber;
                                }
                            }

                            worksheet.Cells[totalRowIndex, sumCol].Value = totalSum;
                            worksheet.Cells[totalRowIndex, sumCol].Style.Font.Bold = true;
                        }

                        var mergeRange = worksheet.Cells[totalRowIndex, startCol, totalRowIndex, startCol + mergeCellIndex - 1];
                        mergeRange.Merge = true;
                        mergeRange.Value = "Tổng số:";
                        mergeRange.Style.Font.Bold = true;
                        mergeRange.Style.Font.Color.SetColor(Color.DarkRed);
                        mergeRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        mergeRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        mergeRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(217, 225, 242));

                        // Định dạng toàn bộ dòng tổng
                        var totalRowRange = worksheet.Cells[totalRowIndex, startCol, totalRowIndex, startCol + dataTable.Columns.Count - 1];
                        totalRowRange.Style.Font.Color.SetColor(Color.DarkRed);
                        totalRowRange.Style.Font.Bold = true;
                        totalRowRange.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        totalRowRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        totalRowRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(217, 225, 242));
                    }
                }

                // Xử lý ẩn cột
                if (hasHiddenCol == true)
                {
                    for (int col = 0; col < dataTable.Columns.Count; col++)
                    {
                        if (dataTable.Columns[col].ColumnName.StartsWith("hidden_"))
                        {
                            worksheet.Column(startCol + col).Hidden = true;
                        }
                    }
                }

                // Xử lý định dạng tùy chỉnh
                if (isCustomCellStyleByValue == true)
                {
                    for (int col = 0; col < dataTable.Columns.Count; col++)
                    {
                        if (dataTable.Columns[col].ColumnName.StartsWith("hidden_"))
                        {
                            int hiddenColIndex = startCol + col;
                            int leftColIndex = hiddenColIndex - 1;

                            if (leftColIndex < startCol)
                            {
                                continue;
                            }

                            for (int row = 0; row < dataTable.Rows.Count; row++)
                            {
                                var compareValue = worksheet.Cells[startRow + 1 + row, hiddenColIndex].Value;

                                if (compareValue != null && decimal.TryParse(compareValue.ToString(), out decimal compareDecimal))
                                {
                                    var cell = worksheet.Cells[startRow + 1 + row, leftColIndex];
                                    if (compareDecimal > 0)
                                    {
                                        cell.Style.Font.Color.SetColor(Color.Red);
                                        cell.Style.Font.Bold = true;
                                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    }
                                    else if (compareDecimal < 0)
                                    {
                                        cell.Style.Font.Color.SetColor(Color.Blue);
                                        cell.Style.Font.Bold = true;
                                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    }
                                    else
                                    {
                                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion


        /// <summary>
        /// Chèn một DataTable vào ExcelWorksheet và giữ nguyên định dạng ban đầu của ô.
        /// </summary>
        /// <param name="worksheet">Worksheet cần thực hiện chèn.</param>
        /// <param name="startHeaderCell">Ô bắt đầu để chèn tiêu đề cột.</param>
        /// <param name="dataTable">DataTable chứa dữ liệu cần chèn.</param>
        /// <param name="hasSumCol">Có thực hiện tính tổng cho các cột không.</param>
        /// <param name="listSumColIndex">Danh sách các chỉ số cột cần tính tổng.</param>
        /// <summary>
        /// Chèn một DataTable vào ExcelWorksheet và giữ nguyên định dạng ban đầu của các cột.
        /// </summary>
        /// <param name="worksheet">Worksheet cần thực hiện chèn.</param>
        /// <param name="startHeaderCell">Ô bắt đầu để chèn tiêu đề cột.</param>
        /// <param name="dataTable">DataTable chứa dữ liệu cần chèn.</param>
        /// <param name="hasSumCol">Có thực hiện tính tổng cho các cột không.</param>
        /// <param name="listSumColIndex">Danh sách các chỉ số cột cần tính tổng.</param>
        public static void InsertDataTable_ByAllTable_KeepFormat(this ExcelWorksheet worksheet, string startHeaderCell, DataTable? dataTable, bool hasSumCol = false, List<int> listSumColIndex = null)
        {
            if (dataTable == null || dataTable.Columns.Count == 0)
            {
                return;
            }

            int startRow = worksheet.Cells[startHeaderCell].Start.Row;
            int startCol = worksheet.Cells[startHeaderCell].Start.Column;

            // Lưu định dạng ban đầu của từng cột (dựa trên hàng ngay dưới tiêu đề)
            var columnFormats = new Dictionary<int, ExcelCellStyle>();
            for (int colIndex = 0; colIndex < dataTable.Columns.Count; colIndex++)
            {
                int currentCol = startCol + colIndex;
                var cell = worksheet.Cells[startRow + 1, currentCol]; // Dựa vào hàng đầu tiên chứa dữ liệu dưới tiêu đề
                if (cell != null && cell.Style != null)
                {
                    columnFormats[colIndex] = new ExcelCellStyle(cell.Style);
                }
            }

            // Chèn tiêu đề cột
            for (int colIndex = 0; colIndex < dataTable.Columns.Count; colIndex++)
            {
                worksheet.Cells[startRow, startCol + colIndex].Value = dataTable.Columns[colIndex].ColumnName;
                worksheet.Cells[startRow, startCol + colIndex].Style.Font.Bold = true;
                worksheet.Cells[startRow, startCol + colIndex].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            }

            // Nếu DataTable có dữ liệu, thực hiện chèn nội dung
            if (dataTable.Rows.Count > 0)
            {
                worksheet.InsertRow(startRow + 1, dataTable.Rows.Count);
                worksheet.Cells[startRow + 1, startCol].LoadFromDataTable(dataTable, false);
            }

            // Khôi phục lại định dạng cho từng cột
            foreach (var format in columnFormats)
            {
                int colIndex = format.Key;
                int col = startCol + colIndex;

                for (int row = startRow + 1; row <= startRow + dataTable.Rows.Count; row++)
                {
                    format.Value.ApplyTo(worksheet.Cells[row, col].Style);
                }
            }

            // Thêm phần tính tổng nếu yêu cầu
            if (hasSumCol && listSumColIndex != null && listSumColIndex.Count > 0)
            {
                int totalRowIndex = startRow + dataTable.Rows.Count + 1;
                int firstSumColIndex = listSumColIndex[0];
                int targetCol = startCol + firstSumColIndex;

                if (firstSumColIndex > 0)
                {
                    var mergeRange = worksheet.Cells[totalRowIndex, startCol, totalRowIndex, targetCol - 1];
                    mergeRange.Merge = true;
                    mergeRange.Value = "Tổng số:";
                    mergeRange.Style.Font.Bold = true;
                    mergeRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    mergeRange.Style.Font.Color.SetColor(Color.FromArgb(139, 0, 0)); // Màu đỏ tối (Dark Red)
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




        // Phương pháp 2: Sử dụng foreach để chèn từng dòng của table vào worksheet
        public static void InsertDataTable_ByEachRow(this ExcelWorksheet worksheet, string startHeaderCell, DataTable? dataTable, bool hasSumCol = false, List<int>? listSumColIndex = null)
        {
            if (dataTable == null)
            {
                return;
            }

            int startRow = worksheet.Cells[startHeaderCell].Start.Row;
            int startCol = worksheet.Cells[startHeaderCell].Start.Column;

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

        #region Group header
        /// <summary>
        /// Tìm các nhóm cột có tên định dạng "groupName<>colName" trong dòng header và tạo header chung cho mỗi nhóm.
        /// Nếu một nhóm chỉ có 1 cột duy nhất, header và cell sẽ được merge thành một và hiển thị ngang.
        /// </summary>
        /// <param name="worksheet">Worksheet Excel</param>
        /// <param name="startCell">Ô bắt đầu của header</param>
        /// <param name="isVertical">Nếu là true, các cell header trong nhóm sẽ xoay dọc và độ rộng cố định; nếu là false, sẽ hiển thị ngang và dùng AutoFit.</param>
        public static void AddGroupHeaders(this ExcelWorksheet worksheet, string startCell, bool isVertical = false)
        {
            var startAddress = worksheet.Cells[startCell].Start;
            int headerRow = startAddress.Row;
            int startColumn = startAddress.Column;

            // Lưu độ rộng các cột trước khi thay đổi
            var columnWidths = new Dictionary<int, double>();
            var hiddenColumns = new HashSet<int>();
            for (int col = startColumn; col <= worksheet.Dimension.End.Column; col++)
            {
                columnWidths[col] = worksheet.Column(col).Width;
                if (worksheet.Column(col).Hidden)
                {
                    hiddenColumns.Add(col);
                }
            }

            // Thêm một dòng mới ở trên dòng header hiện tại để tạo header chung
            worksheet.InsertRow(headerRow, 1);
            int headerGroupRow = headerRow;
            int headerDetailRow = headerRow + 1;

            Regex groupRegex = new Regex(@"^(?<groupName>[^<>]+)<>(?<colName>.+)$");

            int currentColumn = startColumn;
            int totalColumns = worksheet.Dimension.End.Column;
            string currentGroup = null;
            int groupStartColumn = -1;

            while (currentColumn <= totalColumns)
            {
                var cell = worksheet.Cells[headerDetailRow, currentColumn];
                var cellValue = cell.Text?.Trim();

                if (string.IsNullOrEmpty(cellValue))
                {
                    // Bỏ qua cột trống
                    currentColumn++;
                    continue;
                }

                var match = groupRegex.Match(cellValue);

                if (match.Success)
                {
                    string groupName = match.Groups["groupName"].Value;
                    string colName = match.Groups["colName"].Value;

                    if (currentGroup == null || groupName != currentGroup)
                    {
                        if (currentGroup != null && groupStartColumn != -1)
                        {
                            int groupColumnCount = currentColumn - groupStartColumn;
                            if (groupColumnCount == 1)
                            {
                                MergeSingleColumnGroup(worksheet, currentGroup, headerGroupRow, groupStartColumn);
                            }
                            else
                            {
                                CreateGroupHeader(worksheet, currentGroup, headerGroupRow, groupStartColumn, currentColumn - 1, isVertical);
                            }
                        }

                        currentGroup = groupName;
                        groupStartColumn = currentColumn;
                    }

                    cell.Value = colName;
                    cell.Style.Font.Bold = true;
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                    if (isVertical && currentColumn != groupStartColumn)
                    {
                        cell.Style.TextRotation = 90;
                        worksheet.Column(currentColumn).Width = 5;
                    }
                }
                else
                {
                    if (currentGroup != null && groupStartColumn != -1)
                    {
                        int groupColumnCount = currentColumn - groupStartColumn;
                        if (groupColumnCount == 1)
                        {
                            MergeSingleColumnGroup(worksheet, currentGroup, headerGroupRow, groupStartColumn);
                        }
                        else
                        {
                            CreateGroupHeader(worksheet, currentGroup, headerGroupRow, groupStartColumn, currentColumn - 1, isVertical);
                        }
                    }

                    // Chỉ xử lý merge nếu cột có giá trị
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        var cellRange = worksheet.Cells[headerGroupRow, currentColumn, headerDetailRow, currentColumn];
                        cellRange.Merge = true;
                        cellRange.Value = cellValue;
                        cellRange.Style.Font.Bold = true;
                        cellRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        cellRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        cellRange.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }

                    currentGroup = null;
                    groupStartColumn = -1;
                }

                currentColumn++;
            }

            if (currentGroup != null && groupStartColumn != -1)
            {
                int groupColumnCount = currentColumn - groupStartColumn;
                if (groupColumnCount == 1)
                {
                    MergeSingleColumnGroup(worksheet, currentGroup, headerGroupRow, groupStartColumn);
                }
                else
                {
                    CreateGroupHeader(worksheet, currentGroup, headerGroupRow, groupStartColumn, currentColumn - 1, isVertical);
                }
            }

            // Khôi phục độ rộng các cột và trạng thái ẩn
            foreach (var columnWidth in columnWidths)
            {
                worksheet.Column(columnWidth.Key).Width = columnWidth.Value;
                worksheet.Column(columnWidth.Key).Hidden = hiddenColumns.Contains(columnWidth.Key);
            }
        }


        /// <summary>
        /// Tạo header chung cho một dải cột trong cùng một nhóm.
        /// </summary>
        private static void CreateGroupHeader(ExcelWorksheet worksheet, string groupName, int headerRow, int startCol, int endCol, bool isVertical)
        {
            var headerRange = worksheet.Cells[headerRow, startCol, headerRow, endCol];
            headerRange.Merge = true;
            headerRange.Value = groupName;
            headerRange.Style.Font.Bold = true;
            headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            headerRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            headerRange.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

            if (isVertical)
            {
                for (int col = startCol; col <= endCol; col++)
                {
                    worksheet.Column(col).Width = 5;
                    worksheet.Cells[headerRow + 1, col].Style.TextRotation = 90;
                }
            }
            else
            {
                for (int col = startCol; col <= endCol; col++)
                {
                    worksheet.Column(col).AutoFit();
                }
            }
        }

        /// <summary>
        /// Merge cả ô header nhóm với ô header cột duy nhất khi nhóm chỉ có 1 cột và tự động điều chỉnh độ rộng.
        /// </summary>
        private static void MergeSingleColumnGroup(ExcelWorksheet worksheet, string groupName, int headerRow, int column)
        {
            var currentValue = worksheet.Cells[headerRow + 1, column].Value;
            worksheet.Cells[headerRow + 1, column].Value = groupName;
            worksheet.Column(column).AutoFit();
            worksheet.Cells[headerRow + 1, column].Value = currentValue;

            var cell = worksheet.Cells[headerRow, column, headerRow + 1, column];
            cell.Merge = true;
            cell.Value = groupName;
            cell.Style.Font.Bold = true;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cell.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        }

        #endregion Group header

        public class ExcelCellStyle
        {
            public ExcelBorderStyle BorderTop { get; set; }
            public ExcelBorderStyle BorderBottom { get; set; }
            public ExcelBorderStyle BorderLeft { get; set; }
            public ExcelBorderStyle BorderRight { get; set; }
            public ExcelHorizontalAlignment HorizontalAlignment { get; set; }
            public ExcelVerticalAlignment VerticalAlignment { get; set; }
            public bool Bold { get; set; }
            public string NumberFormat { get; set; }

            public ExcelCellStyle(ExcelStyle style)
            {
                BorderTop = style.Border.Top.Style;
                BorderBottom = style.Border.Bottom.Style;
                BorderLeft = style.Border.Left.Style;
                BorderRight = style.Border.Right.Style;
                HorizontalAlignment = style.HorizontalAlignment;
                VerticalAlignment = style.VerticalAlignment;
                Bold = style.Font.Bold;
                NumberFormat = style.Numberformat.Format;
            }

            public void ApplyTo(ExcelStyle style)
            {
                style.Border.Top.Style = BorderTop;
                style.Border.Bottom.Style = BorderBottom;
                style.Border.Left.Style = BorderLeft;
                style.Border.Right.Style = BorderRight;
                style.HorizontalAlignment = HorizontalAlignment;
                style.VerticalAlignment = VerticalAlignment;
                style.Font.Bold = Bold;
                style.Numberformat.Format = NumberFormat;
            }
        }

    }
}

using System.Data;
using System.Reflection;

namespace NencerApi.Extentions
{
    public static class DataTableExtensions
    {
        public static List<T> ToList<T>(this DataTable table) where T : new()
        {
            // Danh sách kết quả
            List<T> list = new List<T>();

            // Lặp qua từng dòng trong DataTable
            foreach (DataRow row in table.Rows)
            {
                T obj = new T();

                foreach (DataColumn column in table.Columns)
                {
                    // Không phân biệt chữ hoa/thường
                    PropertyInfo? property = typeof(T)
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .FirstOrDefault(p => string.Equals(p.Name, column.ColumnName, StringComparison.OrdinalIgnoreCase));

                    if (property != null && row[column] != DBNull.Value)
                    {
                        // Gán giá trị từ DataTable vào thuộc tính của đối tượng
                        property.SetValue(obj, Convert.ChangeType(row[column], property.PropertyType), null);
                    }
                }

                list.Add(obj);
            }

            return list;
        }

    }
}

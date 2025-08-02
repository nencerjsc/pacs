using System;
using System.Text.RegularExpressions;
namespace NencerApi.Helpers
{
    public class SqlHelper
    {
        // Danh sách từ khóa nhạy cảm
        private static readonly string[] RestrictedKeywords = new string[]
        {
        "INSERT", "UPDATE", "DELETE", "DROP", "TRUNCATE", "EXECUTE", "GRANT", "REVOKE", "ALTER", "CREATE", "SHUTDOWN"
            //, "EXEC"
        };

        // Regex để kiểm tra các từ khóa nhạy cảm
        private static readonly string RestrictedKeywordsPattern = $@"\b({string.Join("|", RestrictedKeywords)})\b";

        /// <summary>
        /// Kiểm tra một câu lệnh SQL có hợp lệ không.
        /// </summary>
        /// <param name="sqlQuery">Câu lệnh SQL cần kiểm tra.</param>
        /// <returns>True nếu hợp lệ, False nếu không.</returns>
        public static bool IsValidSqlQuery(string sqlQuery)
        {
            // Kiểm tra nếu chuỗi rỗng hoặc null
            if (string.IsNullOrEmpty(sqlQuery))
            {
                Console.WriteLine("Invalid SQL query: Query is empty.");
                return false;
            }

            // Kiểm tra từ khóa nhạy cảm
            if (Regex.IsMatch(sqlQuery, RestrictedKeywordsPattern, RegexOptions.IgnoreCase))
            {
                // Kiểm tra ngữ cảnh hợp lệ
                if (IsAllowedTemporaryTableOperation(sqlQuery))
                {
                    return true; // Cho phép thao tác hợp lệ trên bảng tạm
                }

                Console.WriteLine("Invalid SQL query: Contains restricted keywords in unsafe context.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Kiểm tra nếu câu lệnh thao tác với bảng tạm là hợp lệ.
        /// </summary>
        /// <param name="sqlQuery">Câu lệnh SQL cần kiểm tra.</param>
        /// <returns>True nếu thao tác hợp lệ, False nếu không.</returns>
        private static bool IsAllowedTemporaryTableOperation(string sqlQuery)
        {
            // Cho phép `INSERT` trên bảng tạm
            if (Regex.IsMatch(sqlQuery, @"INSERT\s+INTO\s+#", RegexOptions.IgnoreCase))
            {
                return true;
            }

            // Cho phép `UPDATE` trên bảng tạm
            if (Regex.IsMatch(sqlQuery, @"UPDATE\s+#", RegexOptions.IgnoreCase))
            {
                return true;
            }

            // Cho phép `DELETE` trên bảng tạm
            if (Regex.IsMatch(sqlQuery, @"DELETE\s+FROM\s+#", RegexOptions.IgnoreCase))
            {
                return true;
            }

            // Cho phép `CREATE`, `DROP`, hoặc `TRUNCATE` trên bảng tạm
            if (Regex.IsMatch(sqlQuery, @"CREATE\s+TABLE\s+#", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(sqlQuery, @"DROP\s+TABLE\s+#", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(sqlQuery, @"TRUNCATE\s+TABLE\s+#", RegexOptions.IgnoreCase))
            {
                return true;
            }

            return false; // Mặc định không cho phép
        }
    }

}

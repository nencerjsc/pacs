using NencerApi.Extentions;
using System.Globalization;
using System.Text.RegularExpressions;

namespace NencerApi.Helpers
{
    public static class NumberHelper
    {
        public static string ToCurrencyString(this decimal? value)
        {
            return value.GetValueOrDefault().ToString("#,##0.00", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Chuyển đổi số tiền thành tiền chữ
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu số</typeparam>
        /// <param name="amount">Số tiền</param>
        /// <returns>Tiền chữ</returns>
        public static string ToAmoutVietnameWords<T>(this T amount) where T : IComparable, IConvertible, IFormattable
        {
            // Kiểm tra nếu amount bằng 0
            if (EqualityComparer<T>.Default.Equals(amount, default))
                return "Không đồng";

            string[] unitArray = { "", " nghìn", " triệu", " tỷ" };
            string[] numberArray = { "", " một", " hai", " ba", " bốn", " năm", " sáu", " bảy", " tám", " chín" };

            string[] unitArrayWithHundred = { "", " trăm", " nghìn", " triệu", " tỷ" };
            string[] numberArrayWithTen = { "", " mười", " hai mươi", " ba mươi", " bốn mươi", " năm mươi", " sáu mươi", " bảy mươi", " tám mươi", " chín mươi" };

            string result = "";
            int i = 0;

            // Sử dụng phương thức ToInt64 để chuyển đổi amount thành kiểu long
            long longAmount = Convert.ToInt64(amount);

            while (longAmount > 0)
            {
                int group = (int)(longAmount % 1000);
                longAmount /= 1000;

                if (group > 0)
                {
                    string groupResult = "";

                    if (group >= 100)
                    {
                        int digit = group / 100;
                        groupResult += numberArray[digit] + unitArrayWithHundred[1];
                        group %= 100;

                        if (group > 0)
                            groupResult += " ";
                    }

                    if (group >= 10)
                    {
                        int digit = group / 10;
                        groupResult += numberArrayWithTen[digit];
                        group %= 10;

                        if (group > 0)
                            groupResult += " ";
                    }

                    if (group > 0)
                        groupResult += numberArray[group];

                    result = groupResult + unitArray[i] + result;
                }

                i++;
            }

            result = result + " đồng";

            // Thực hiện replace sử dụng regex
            string result2 = Regex.Replace(result, "\\s+", " ");

            return result2.Trim().ToCapEachWord();
        }
    }
}

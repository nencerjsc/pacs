namespace NencerApi.Extentions
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Lấy thông điệp chi tiết của Exception, bao gồm cả InnerException nếu có.
        /// </summary>
        /// <param name="ex">Exception cần lấy thông tin.</param>
        /// <returns>Thông điệp chi tiết của Exception.</returns>
        public static string GetDetailMess(this Exception ex)
        {
            if (ex == null) return string.Empty;

            // Lấy thông điệp của InnerException nếu có
            var innerExceptionMessage = ex.InnerException != null
                ? $" | Inner Exception: {ex.InnerException.Message}"
                : string.Empty;

            // Kết hợp thông điệp chính và InnerException
            var detailedMessage = $"Exception: {ex.Message}{innerExceptionMessage} ";

            return detailedMessage;
        }
    }
}

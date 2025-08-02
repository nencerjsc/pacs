using System;
using System.Collections.Generic;
namespace NencerApi.Helpers
{
    public class BaseResponse<T>
    {
        public string Status { get; set; } = "200";
        public string Message { get; set; }
        public T? Data { get; set; }

        public BaseResponse() { } //hàm tạo mặc định dùng cho các lớp kế thừa
        public BaseResponse(T data)
        {
            Data = data;
        }

        public BaseResponse(string responseCode, string message, T? data)
        {
            Status = responseCode;
            Message = message;
            Data = data;
        }
    }

    public class ExceptionErrorResponse<T> : BaseResponse<T>
    {
        public ExceptionErrorResponse(Exception ex, string? message = "")
        {
            //lấy message dễ hiểu và trọng tâm để response
            var innerExceptionMessage = ex.InnerException != null ? $" | Inner Exception: {ex.InnerException.Message}" : string.Empty;
            var detailedMessage = $"Exception: {ex.Message}{innerExceptionMessage} ";

            Status = "500";
            Message = detailedMessage;
            //ghi log
            Helpers.LogHelper.Exception(message, ex);
        }
    }

    public class BaseResponseList<T>
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }

        public BaseResponseList(string status, string message, T? data, int pageNumber = 1, int pageSize = 10, int totalPages = 1, int totalItems = 0)
        {
            Status = status;
            Message = message;
            Data = data;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = totalPages;
            TotalItems = totalItems;
        }
    }

    public class ErrorResponse<T> : BaseResponse<T>
    {
        public ErrorResponse() { Status = "500"; Message = "Có lỗi xảy ra"; }

        public ErrorResponse(string message)
        {
            Status = "500";
            Message = message;
        }

        public ErrorResponse(string responseCode, string message)
        {
            Status = responseCode;
            Message = message;
        }
    }

    public class NoPermissionErrorResponse<T> : BaseResponse<T>
    {
        public NoPermissionErrorResponse() { Status = "403"; Message = "required_permission"; }
    }

    public class BadRequestResponse<T> : BaseResponse<T>
    {
        public BadRequestResponse() { Status = "400"; Message = "bad_request"; }
        public BadRequestResponse(string mess) { Status = "400"; Message = mess; }

    }

}

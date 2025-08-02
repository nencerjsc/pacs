using Microsoft.AspNetCore.Mvc.Filters;
using NencerApi.Helpers;

namespace NencerApi.ActionFilter
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            LogHelper.Exception(context.Exception.Message, context.Exception);
        }
    }
}

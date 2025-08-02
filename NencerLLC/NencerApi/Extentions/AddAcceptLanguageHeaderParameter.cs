using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NencerApi.Extentions
{
    public class AddAcceptLanguageHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Accept-Language",
                In = ParameterLocation.Header,
                Required = false, // Change this to true if you want it to be required
                Schema = new OpenApiSchema
                {
                    Type = "string",
                }
            });
        }
    }
}

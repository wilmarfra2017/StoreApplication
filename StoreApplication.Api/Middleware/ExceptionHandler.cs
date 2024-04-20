using Microsoft.AspNetCore.Diagnostics;
using StoreApplication.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace StoreApplication.Api.Middleware;

public static class ExceptionHandler
{
    public static void UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(appBuilder =>
        {
            appBuilder.Run(async context =>
            {
                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (exceptionHandlerFeature != null)
                {
                    var ex = exceptionHandlerFeature.Error;
                    var code = HttpStatusCode.InternalServerError; 

                    if (ex is LoginManagementException) code = HttpStatusCode.BadRequest;
                    else if (ex is UnauthorizedAccessException) code = HttpStatusCode.Unauthorized;

                    context.Response.StatusCode = (int)code;
                    context.Response.ContentType = "application/json";

                    var result = JsonSerializer.Serialize(new { error = ex.Message });
                    await context.Response.WriteAsync(result);
                }
            });
        });
    }
}

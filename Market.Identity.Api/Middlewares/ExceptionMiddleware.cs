using System.Net;
using System.Text.Json;
using Market.Identity.Application;

namespace Market.Identity.Middlewares;

public class ExceptionMiddleware(ILogger<ExceptionMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Произошла неожиданная ошибка");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            
            var result = Result<object>.Failure(e.Message);
            var json = JsonSerializer.Serialize(result);    
            
            await context.Response.WriteAsync(json);
        }
    }
}
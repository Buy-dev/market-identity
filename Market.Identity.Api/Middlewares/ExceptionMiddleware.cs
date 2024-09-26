using System.Net;
using System.Text.Json;
using Market.Identity.Application;

namespace Market.Identity.Middlewares;

public class ExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            
            var result = Result<object>.Failure(e.Message);
            var json = JsonSerializer.Serialize(result);    
            
            await context.Response.WriteAsync(json);
        }
    }
}
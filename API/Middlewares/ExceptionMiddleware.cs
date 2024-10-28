using System;
using System.Text.Json;
using API.Errors;

namespace API.Middlewares;

public class ExceptionMiddleware(IHostEnvironment env, RequestDelegate next)
{
    private static readonly JsonSerializerOptions JsonSerializerOptions =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            await HandleExceptionAsync(context, e, env);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception e, IHostEnvironment env)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 500;

        ApiErrorResponse response =
            new(context.Response.StatusCode, e.Message, "Internal Server Error");

        if (env.IsDevelopment())
        {
            response.Details = e.StackTrace;
        }

        var json = JsonSerializer.Serialize(response, JsonSerializerOptions);

        return context.Response.WriteAsync(json);
    }
}

using Business.Abstract;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Middleware;

public class ModelValidationMiddleware
{
    private readonly RequestDelegate _next;

    public ModelValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IErrorLogService errorLogsService)
    {
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;
        var userName = context.User.Identity?.Name ?? null;

        context.Request.EnableBuffering();
        var requestBodyStream = new MemoryStream();
        await context.Request.Body.CopyToAsync(requestBodyStream);
        requestBodyStream.Seek(0, SeekOrigin.Begin);
        var requestBodyText = await new StreamReader(requestBodyStream).ReadToEndAsync();
        context.Request.Body.Seek(0, SeekOrigin.Begin);

        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);

            if (context.Response.StatusCode == 400)
            {
                responseBody.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(responseBody).ReadToEndAsync();
                responseBody.Seek(0, SeekOrigin.Begin);

                errorLogsService.LogError(new Exception(responseText), requestBodyText, userName, requestMethod, requestPath);
            }
        }
        finally
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
        }
    }
}
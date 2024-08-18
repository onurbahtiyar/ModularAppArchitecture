using DataAccess.Abstract;
using DataAccess.Concrete.Dapper;
using Entities.Concrete.EntityFramework.Entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text;

namespace Infrastructure.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IUserActivityLogDal userActivityLogDal;
    private readonly MiddlewareRepository middlewareRepository;
    private readonly CacheService cacheService;
    private readonly IErrorLogDal errorLogDal;
    private readonly LogErrorRepository logErrorRepository;

    public RequestLoggingMiddleware(RequestDelegate next, IUserActivityLogDal userActivityLogDal, CacheService cacheService, IErrorLogDal errorLogDal, MiddlewareRepository middlewareRepository, LogErrorRepository logErrorRepository)
    {
        _next = next;
        this.userActivityLogDal = userActivityLogDal;
        this.cacheService = cacheService;
        this.errorLogDal = errorLogDal;
        this.middlewareRepository = middlewareRepository;
        this.logErrorRepository = logErrorRepository;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (
            !context.Request.Path.Value.EndsWith("/isMaintenanceMode", StringComparison.OrdinalIgnoreCase) &&
            !context.Request.Path.Value.EndsWith("/GetPermsControl", StringComparison.OrdinalIgnoreCase))
        {
            await Task.Run(() => LogRequestAsync(context));
        }

        await _next(context);
    }

    private async Task LogRequestAsync(HttpContext context)
    {
        try
        {
            string userInput = null;

            try
            {
                if (context.Request.Method == HttpMethod.Get.Method)
                {
                    var queryParams = context.Request.Query
                        .ToDictionary(p => p.Key, p => p.Value.ToString());
                    userInput = JsonConvert.SerializeObject(queryParams);
                }
                else if (context.Request.Method == HttpMethod.Post.Method)
                {
                    context.Request.EnableBuffering();
                    using (var reader = new StreamReader(context.Request.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true))
                    {
                        userInput = await reader.ReadToEndAsync();
                        context.Request.Body.Position = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                logErrorRepository.LogError(ex, new { detail = "userInput error" }, null, "LogRequestAsync", "RequestLoggingMiddleware");
            }


            try
            {

            }
            catch (Exception ex)
            {
                logErrorRepository.LogError(ex, null, Guid.Empty, "LogRequestAsync", "RequestLoggingMiddleware");
            }
            var userId = context.User?.Identity != null ? Guid.Parse(context.User?.Identity.Name) : Guid.Empty;

            string hostname = "Unkown";
            hostname = context.Connection.RemoteIpAddress?.ToString();

            var newLog = new UserActivityLog
            {
                Ipaddress = context.Connection.RemoteIpAddress?.ToString(),
                BrowserInfo = context.Request.Headers["User-Agent"].ToString(),
                ActivityDate = DateTime.Now,
                ActivityType = context.Request.Method,
                ActivityDetail = context.Request.Path,
                UserGuid = userId,
                AdditionalData = JsonConvert.SerializeObject(userInput) != null ? JsonConvert.SerializeObject(userInput) : null,
                ActivityPage = context.Request.Headers["User-Page"].FirstOrDefault()
            };

            await middlewareRepository.AddUserActivityLogAsync(newLog);
        }
        catch (Exception ex)
        {
            logErrorRepository.LogError(ex, new { detail = "LogRequestAsync error" }, null, "LogRequestAsync", "RequestLoggingMiddleware");
        }
    }
}
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace Infrastructure.Middleware
{
    public class MemoryRateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ConcurrentDictionary<string, (RateLimitInfo, SemaphoreSlim)> _rateLimits = new ConcurrentDictionary<string, (RateLimitInfo, SemaphoreSlim)>();

        private readonly int _requestLimit;
        private readonly TimeSpan _timePeriod;

        public MemoryRateLimitingMiddleware(RequestDelegate next, int requestLimit, TimeSpan timePeriod)
        {
            _next = next;
            _requestLimit = requestLimit;
            _timePeriod = timePeriod;
        }

        public async Task Invoke(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            if (!context.Request.Path.Value?.EndsWith("Auth/isMaintenanceMode", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                if (ipAddress != "unknown")
                {
                    var (rateLimitInfo, semaphore) = _rateLimits.GetOrAdd(ipAddress, _ => (new RateLimitInfo(), new SemaphoreSlim(1, 1)));

                    await semaphore.WaitAsync();
                    try
                    {
                        if (rateLimitInfo.RequestCount >= _requestLimit &&
                            rateLimitInfo.WindowStart.Add(_timePeriod) > DateTime.UtcNow)
                        {
                            context.Response.ContentType = "application/json";
                            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                            var response = new { data = null as object, message = "Rate limit exceeded", success = false };
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                            return;
                        }

                        if (rateLimitInfo.WindowStart.Add(_timePeriod) <= DateTime.UtcNow)
                        {
                            rateLimitInfo.Reset(DateTime.UtcNow);
                        }

                        rateLimitInfo.RequestCount++;
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }
            }

            await _next(context);
        }

        private class RateLimitInfo
        {
            public DateTime WindowStart { get; private set; }
            public int RequestCount { get; set; }

            public RateLimitInfo()
            {
                Reset(DateTime.UtcNow);
            }

            public void Reset(DateTime currentTime)
            {
                WindowStart = currentTime;
                RequestCount = 0;
            }
        }
    }
}
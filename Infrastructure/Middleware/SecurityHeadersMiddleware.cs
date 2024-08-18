using Microsoft.AspNetCore.Http;

namespace Infrastructure.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.Headers.Remove("Server");
            context.Response.Headers.Remove("X-Powered-By");

            if (!context.Response.Headers.ContainsKey("X-XSS-Protection"))
            {
                context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
            }

            if (!context.Response.Headers.ContainsKey("X-Content-Type-Options"))
            {
                context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            }

            if (!context.Response.Headers.ContainsKey("Referrer-Policy"))
            {
                context.Response.Headers.Append("Referrer-Policy", "no-referrer");
            }

            if (!context.Response.Headers.ContainsKey("Permissions-Policy"))
            {
                context.Response.Headers.Append("Permissions-Policy", "geolocation=(), microphone=(), camera=()");
            }

            if (!context.Response.Headers.ContainsKey("Content-Security-Policy"))
            {
                context.Response.Headers.Append("Content-Security-Policy",
                    "default-src 'self'; " +
                    "script-src 'self' 'unsafe-inline'; " +
                    "style-src 'self' 'unsafe-inline'; " +
                    "img-src 'self' data:; " +
                    "font-src 'self'; " +
                    "frame-src 'self';");
            }

            await _next(context);
        }
    }
}
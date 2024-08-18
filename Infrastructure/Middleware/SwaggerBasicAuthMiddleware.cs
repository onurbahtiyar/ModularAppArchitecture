using Microsoft.AspNetCore.Http;
using System.Text;

namespace Infrastructure.Middleware
{
    public class SwaggerBasicAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public SwaggerBasicAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                string authHeader = context.Request.Headers["Authorization"];
                if (authHeader == null || !authHeader.StartsWith("Basic "))
                {
                    context.Response.StatusCode = 401;
                    context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Swagger\"";
                    return;
                }
                else
                {
                    var encoding = Encoding.GetEncoding("iso-8859-1");
                    var usernamePassword = encoding.GetString(Convert.FromBase64String(authHeader.Substring(6))).Split(':');
                    var username = usernamePassword[0];
                    var password = usernamePassword[1];

                    if ((username != "cxoder" || password != "asc16rr5") && (username != "cigdem" || password != "19BB03jk") && (username != "cagla.kirazkaya" || password != "Asdf1234."))
                    {
                        context.Response.StatusCode = 401;
                        return;
                    }
                }
            }

            await _next.Invoke(context);
        }
    }
}
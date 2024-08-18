using Autofac;
using Autofac.Extensions.DependencyInjection;
using Business.DependencyRepository.Autofac;
using Core.Utilities.Security.Encryption;
using Core.Utilities.Security.JWT;
using Enigma;
using Infrastructure.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Security.Claims;
using System.Security.Cryptography;

internal class Program
{
    private static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.File("logs/error_.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = null;
        });

        builder.Host.UseSerilog();

        DataAccess.Concrete.Dapper.Context.ContextDb.Configure(builder.Configuration);

        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        builder.Host.ConfigureContainer<ContainerBuilder>(builder => builder.RegisterModule(new AutofacBusinessModule()));

        IConfiguration configuration = builder.Configuration;

        var aesSettings = builder.Configuration.GetSection("AesSettings").Get<AesSettings>();
        KeyCase.Instance.SetAesKeys(aesSettings.Key, aesSettings.Vektor);
        builder.Services.AddSingleton(KeyCase.Instance);

        builder.WebHost.UseKestrel();
        builder.WebHost.UseIIS();

        builder.Services.AddControllers();

        Enigma.Processor processor = new Enigma.Processor();

        var projectName = configuration["ProjectSettings:ProjectName"];

        var encryptedDefaultConnection = configuration["ConnectionStrings:DatabaseConnection"];
        var encryptedsecurityKey = configuration["JwtSettings:SecurityKey"];

        string connectionString = "";
        string securityKey = "";

        try
        {
            using (Aes aes = Aes.Create())
            {
                connectionString = processor.DecryptorSymmetric(encryptedDefaultConnection, aes);
                securityKey = processor.DecryptorSymmetric(encryptedsecurityKey, aes);
            }
        }
        catch (FormatException fe)
        {
            Console.WriteLine("Şifre çözme işlemi sırasında format hatası: " + fe.Message);
        }
        catch (CryptographicException ce)
        {
            Console.WriteLine("Kriptografik işlem sırasında hata: " + ce.Message);
        }

        builder.Services.AddDbContext<Entities.Concrete.EntityFramework.Context.ContextDb>(options => options.UseSqlServer(connectionString));

        builder.Services.AddScoped<SqlConnection>(sp => new SqlConnection(DataAccess.Concrete.Dapper.Context.ContextDb.ConnectionStringDefault));

        builder.Services.AddMemoryCache();
        builder.Services.AddTransient<CacheService>();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddHttpClient();

        var corsList = new List<string>();

        var corsOrigins = configuration.GetSection("CorsOrigins").Get<List<string>>();
        var parsedCorsOrigins = corsOrigins.Select(url => new Uri(url)).ToList();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("CustomCorsPolicy", builder =>
            {
                builder
                    .SetIsOriginAllowed(origin =>
                    {
                        Uri parsedOrigin = new Uri(origin);
                        return parsedCorsOrigins.Any(corsOrigin =>
                            corsOrigin.Scheme == parsedOrigin.Scheme &&
                            corsOrigin.Host == parsedOrigin.Host &&
                            corsOrigin.Port == parsedOrigin.Port);
                    })
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        var tokenOptions = configuration.GetSection("TokenOptions").Get<TokenOptions>();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = tokenOptions.Issuer,
                ValidAudience = tokenOptions.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(securityKey),
                ClockSkew = TimeSpan.Zero,
                RoleClaimType = ClaimTypes.Role
            };
        });

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = $"{projectName} API", Version = "v1" });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    new string[] {}
                }
            });
        });


        builder.Services.AddControllers(options =>
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            options.Filters.Add(new AuthorizeFilter(policy));
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddAutoMapper(typeof(Program).Assembly);

        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        var app = builder.Build();

#if DEVELOPER
        builder.WebHost.UseSetting("detailedErrors", "true");
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.RoutePrefix = projectName.Trim();
            c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{projectName} API v1");
            c.InjectStylesheet("/MainFile/swagger-mycustom.css");
        });

        app.UseWhen(context => context.Request.Path.StartsWithSegments("/swagger"), appBuilder =>
        {
            appBuilder.UseMiddleware<SwaggerBasicAuthMiddleware>();
        });
#else
        builder.WebHost.UseSetting("detailedErrors", "false");
#endif

        builder.WebHost.CaptureStartupErrors(true);

        #region MainFile Settings

        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "MainFile")))
        {
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "MainFile"));
        }

        app.UseStaticFiles();

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "MainFile")),
            RequestPath = "/MainFile"
        });

        #endregion MainFile Settings

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors("CustomCorsPolicy");
        app.UseAuthentication();
        app.UseAuthorization();

        var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.Use(async (context, next) =>
        {
            await next();
            if (context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value))
            {
                context.Request.Path = "/index.html";
                await next();
            }
        });

        app.UseMiddleware<MemoryRateLimitingMiddleware>(150, TimeSpan.FromMinutes(1));
        app.UseMiddleware<ModelValidationMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<SecurityHeadersMiddleware>();

        app.MapControllers();
        app.Run();
    }
}
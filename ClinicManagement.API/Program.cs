using ClinicManagement.API;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.HttpOverrides;

namespace ClinicManagement.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            
            builder.Services.AddDependansiesServices(builder.Configuration);

            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy
                        .WithOrigins(
                            "http://127.0.0.1:5500",          // Local Frontend
                            "http://localhost:5173",          // Local React App
                            "http://127.0.0.1:5173",          // Local React App IP
                            "https://mohamed68909.github.io", // GitHub Pages
                            "https://ClinicManagement.netlify.app")  // Netlify
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            builder.Services.AddRateLimiter(options =>
            {
                // Fix: Use OnRejected callback instead of a manual middleware after UseRateLimiter().
                // Writing to the response after UseRateLimiter() causes an InvalidOperationException
                // because the response has already started.
                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.HttpContext.Response.ContentType = "text/plain";
                    await context.HttpContext.Response.WriteAsync(
                        "Too many login attempts. Please try again later.",
                        cancellationToken);
                };

                options.AddPolicy("AuthLimiter", httpContext =>
                {
                    // Use X-Forwarded-For header if available (handles reverse proxies)
                    var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: ip,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        });
                });
            });
            var app = builder.Build();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            
            app.UseCors("AllowFrontend");

            app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
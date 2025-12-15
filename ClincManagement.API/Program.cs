using ClincManagement.API;

namespace ClincManagement.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services
            builder.Services.AddDependansiesServices(builder.Configuration);
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ? CORS Configuration
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    policy =>
                    {
                        policy
                            .WithOrigins(
                                "http://127.0.0.1:5500",           // Local Frontend
                                "https://mohamed68909.github.io", // GitHub Pages
                        "https://curexmind.netlify.app")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            var app = builder.Build();

            // Swagger
          
                app.UseSwagger();
                app.UseSwaggerUI();
            

            app.UseHttpsRedirection();

            // ? Important: CORS before Authorization
            app.UseCors("AllowFrontend");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}

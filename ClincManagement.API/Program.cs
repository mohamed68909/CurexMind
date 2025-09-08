using Mapster;

namespace ClincManagement.API
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDependansiesServices(builder.Configuration);

           

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

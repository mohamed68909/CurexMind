using ClincManagement.API.Services.Interface;
using CurexMind.API.Services.Interface;
using CurexMind.API.Services;
using Mapster;
using MapsterMapper;
using System.Reflection;
using ClincManagement.API.Services;
using HospitalManagement.API.Services;

namespace ClincManagement.API.Extentions
{
    public static class DI
    {
        public static IServiceCollection AddDependansiesServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddConnectionConfig(configuration);
            services.AddControllerConfig();
            services.AddMapsterConfig();
            services.AddRegistrationConfig();

            return services;
        }
        private static IServiceCollection AddConnectionConfig(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                  throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            

            return services;
        }
        private static IServiceCollection AddControllerConfig(this IServiceCollection services)
        {
            services.AddControllers();
            return services;
        }
        private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
        {
            var mapsterConfig = TypeAdapterConfig.GlobalSettings;
            mapsterConfig.Scan(Assembly.GetExecutingAssembly());
            services.AddSingleton<IMapper>(new Mapper(mapsterConfig));
            return services;
        }
        private static IServiceCollection AddRegistrationConfig(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<IAppointmentService, AppointmentService>();






            return services;
        }
    }
}

using ClincManagement.API.Helpers;
using ClincManagement.API.Services;
using ClincManagement.API.Services.Interface;
using ClincManagement.API.Settings;
using ClinicManagement.API.Services;
using ClinicManagement.API.Services.Interface;
using CurexMind.API.Services;
using CurexMind.API.Services.Interface;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

namespace ClincManagement.API.Extentions
{
    public static class DI
    {
        public static IServiceCollection AddDependansiesServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddConnectionConfig(configuration);
            services.AddControllerConfig();
            services.AddMapsterConfig();
            services.AddRegistrationConfig();

            services.AddEndpointsApiExplorer();

           
            services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(type => type.FullName);
            });

            services.AddAuthenticationConfig(configuration);

            return services;
        }

        private static IServiceCollection AddConnectionConfig(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                  throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 10,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                }));

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

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

        private static IServiceCollection AddAuthenticationConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services
               .AddOptions<JwtOptions>()
               .BindConfiguration(JwtOptions.SectionName)
               .ValidateDataAnnotations()
               .ValidateOnStart();

            var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ??
                throw new InvalidOperationException($"Configuration section '{JwtOptions.SectionName}' not found or invalid.");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
                };
            });

            return services;
        }

        private static IServiceCollection AddRegistrationConfig(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IUserHelpers, UserHelpers>();
           // services.AddScoped<IStayService, StayService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IImageFileService, ImageFileService>();

            return services;
        }
    }
}

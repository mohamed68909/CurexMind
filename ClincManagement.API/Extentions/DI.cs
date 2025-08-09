namespace ClincManagement.API.Extentions
{
    public static class DI
    {
        public static IServiceCollection AddDependansiesServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddConnectionConfig(configuration);

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
    }
}

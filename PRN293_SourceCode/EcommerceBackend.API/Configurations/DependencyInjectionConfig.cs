using EcommerceBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;


namespace EcommerceBackend.API.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<EcommerceDBContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure();
                }
                ));
            // Register any Services and Repositories
            //services.AddScoped<IUserService, UserService>();
            //services.AddScoped<IAuthService, AuthService>();
            //services.AddScoped<IEmailService, EmailService>();
            //services.AddScoped<IOtpService, OtpService>();
            //services.AddScoped<IUserRepository, UserRepository>();

        }
    }
}

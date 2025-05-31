using EcommerceBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using EcommerceBackend.BusinessObject.Abstract;
using EcommerceBackend.BusinessObject.Services;
using EcommerceBackend.DataAccess.Abstract;
using EcommerceBackend.DataAccess.Repository;
using EcommerceBackend.BusinessObject.Services.ProductService;

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

            // Register Repositories
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductService, ProductService>();
            // Register Services
            //services.AddScoped<IAuthService, AuthService>();


            // Register other services as needed
            //services.AddScoped<IUserService, UserService>();
            //services.AddScoped<IEmailService, EmailService>();
            //services.AddScoped<IOtpService, OtpService>();
        }
    }
}

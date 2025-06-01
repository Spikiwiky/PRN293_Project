using EcommerceBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

using EcommerceBackend.DataAccess.Abstract.AuthAbstract;
using EcommerceBackend.DataAccess.Repository.AuthRepository;
using EcommerceBackend.BusinessObject.Abstract.AuthAbstract;
using EcommerceBackend.BusinessObject.Services.AuthService;
using EcommerceBackend.DataAccess.Repository;
using EcommerceBackend.BusinessObject.Services;
using EcommerceBackend.BusinessObject.Services.ProductService;
using EcommerceBackend.DataAccess.Abstract;


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

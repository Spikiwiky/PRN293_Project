using EcommerceBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using EcommerceBackend.DataAccess.Abstract.AuthAbstract;
using EcommerceBackend.DataAccess.Repository.AuthRepository;
using EcommerceBackend.BusinessObject.Abstract.AuthAbstract;
using EcommerceBackend.BusinessObject.Services.AuthService;
using EcommerceBackend.DataAccess.Abstract;
using EcommerceBackend.DataAccess.Repository;
using EcommerceBackend.BusinessObject.Services;
using EcommerceBackend.BusinessObject.Services.OrderService;
using EcommerceBackend.BusinessObject.Abstract;
using EcommerceBackend.BusinessObject.Services.CartService;
using EcommerceBackend.DataAccess.Repository.CartRepository;

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

            // Register Dependency injection
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<DataAccess.Repository.IProductRepository, ProductRepository>();
            services.AddScoped<IProductService, ProductService>();
           
            //Register for auth - Moved to Program.cs
            //services.AddScoped<IAuthService, AuthService>();
            //services.AddScoped<IAuthRepository, AuthRepository>();

            //Register for cart - Moved to Program.cs
            //services.AddScoped<ICartService, CartService>();
            //services.AddScoped<ICartRepository, CartRepository>();

            //Register for order
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderRepository, OrderRepository>();

            // Register other services as needed
            //services.AddScoped<IUserService, UserService>();
            //services.AddScoped<IEmailService, EmailService>();
            //services.AddScoped<IOtpService, OtpService>();
        }
    }
}

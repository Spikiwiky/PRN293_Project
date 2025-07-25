using Microsoft.EntityFrameworkCore;
using EcommerceBackend.API.Configurations;
using EcommerceBackend.API.Hubs;
using EcommerceBackend.BusinessObject.Services;
using EcommerceBackend.DataAccess.Abstract.BlogAbstract;
using EcommerceBackend.DataAccess.Repository.BlogRepository;
using EcommerceBackend.BusinessObject.Services.UserService;
using EcommerceBackend.DataAccess.Repository.UserRepository;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using EcommerceBackend.DataAccess.Models;


using EcommerceBackend.BusinessObject.Services.SaleService;
using EcommerceBackend.DataAccess.Abstract;
using EcommerceBackend.DataAccess.Repository;
using EcommerceBackend.BusinessObject.Abstract;
using EcommerceBackend.BusinessObject.Services.CartService;
using EcommerceBackend.DataAccess.Repository.CartRepository;
using EcommerceBackend.DataAccess.Abstract.AuthAbstract;
using EcommerceBackend.DataAccess.Repository.AuthRepository;
using EcommerceBackend.BusinessObject.Abstract.AuthAbstract;
using EcommerceBackend.BusinessObject.Services.AuthService;


using EcommerceBackend.DataAccess.Repository.SaleRepository.SaleCategory;
using EcommerceBackend.BusinessObject.Services.SaleService.CategoryService;
using EcommerceBackend.BusinessObject.Services.SaleService.ProductService;
using EcommerceBackend.DataAccess.Repository.SaleRepository.OrderRepo;
using EcommerceBackend.BusinessObject.Services.SaleService.OrderService;
using EcommerceBackend.BusinessObject.Services.SaleService.UserService;
using EcommerceBackend.DataAccess.Repository.SaleRepository.UserRepo;
using EcommerceBackend.BusinessObject.Services.SaleService.CategoryService.CategoryService;
using EcommerceBackend.DataAccess.Repository.SaleRepository.BlogRepo;
using EcommerceBackend.BusinessObject.Services.SaleService.BlogService;
using EcommerceBackend.BusinessObject.Abstract;
using EcommerceBackend.BusinessObject.Services.CartService;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IBlogRepository, BlogRepository>();
builder.Services.AddScoped<BlogService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();


//thanhvv
builder.Services.AddDbContext<EcommerceDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<EcommerceBackend.DataAccess.Repository.IProductRepository, EcommerceBackend.DataAccess.Repository.ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

//builder.Services.AddScoped<IOrderRepository, OrderRepository>();
//builder.Services.AddScoped<IOrderService, OrderService>();



//builder.Services.AddScoped<ISaleProductService, SaleProductService>();

builder.Services.AddScoped<ISaleService, SaleService>(); 
// builder.Services.AddScoped<EcommerceBackend.DataAccess.Repository.SaleRepository.IProductRepository, EcommerceBackend.DataAccess.Repository.SaleRepository.ProductRepository>();
// builder.Services.AddScoped<EcommerceBackend.BusinessObject.Services.SaleService.ICategoryService, EcommerceBackend.BusinessObject.Services.SaleService.CategoryService>();
// builder.Services.AddScoped<EcommerceBackend.DataAccess.Repository.SaleRepository.ICategoryRepository, EcommerceBackend.DataAccess.Repository.SaleRepository.CategoryRepository>();

// Register Cart Services
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICartRepository, CartRepository>();

// Register Auth Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();


builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<EcommerceBackend.DataAccess.Repository.SaleRepository.ProductRepo.IProductRepository, EcommerceBackend.DataAccess.Repository.SaleRepository.ProductRepo.ProductRepository>();
builder.Services.AddScoped<ISaleCategoryService, SaleCategoryService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ISaleOrderService, SaleOrderService>();
builder.Services.AddScoped<ISaleOrderRepository, SaleOrderRepository>();
builder.Services.AddScoped<ISaleUserService, SaleUserService>();
builder.Services.AddScoped<ISaleUserRepository, SaleUserRepository>();
builder.Services.AddScoped<ISaleBlogRepository, SaleBlogRepository>();
builder.Services.AddScoped<ISaleBlogService, SaleBlogService>();
builder.Services.AddScoped<ICartService, CartService>();

// Config Authentication Jwt
JwtConfig.ConfigureJwtAuthentication(builder.Services, builder.Configuration);
JwtConfig.ConfigureSwagger(builder.Services);
// Add Application Services (custom config DI)
builder.Services.AddApplicationServices(builder.Configuration);

//session
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = builder.Configuration["GoogleKeys:ClientId"];
    options.ClientSecret = builder.Configuration["GoogleKeys:ClientSecret"];
});

//builder.Services.AddControllers()
//    .AddJsonOptions(x =>
//        x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve);


builder.Services.AddAuthorization();

//builder.Services.AddCorsPolicy(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<SignalrHub>("/SignalrHub");

app.UseSwagger();
app.UseSwaggerUI();
app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
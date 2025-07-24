using EcommerceFrontend.Web.Services;
using EcommerceFrontend.Web.Services.User;

using EcommerceFrontend.Web.Services.Blog;
using EcommerceFrontend.Web.Services.Sale;
using EcommerceFrontend.Web.Models.Sale;
using EcommerceFrontend.Web.Services.AI;
using EcommerceFrontend.Web.Service.AI;
using EcommerceFrontend.Web.Services.Admin.BlogCategory;
using BlogCategoryService = EcommerceFrontend.Web.Services.Admin.BlogCategory.BlogCategoryService;
using EcommerceFrontend.Web.Services.Admin.Blog;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Register HTTP client services
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];
if (string.IsNullOrEmpty(apiBaseUrl))
{
    throw new ArgumentNullException("ApiSettings:BaseUrl is not configured in appsettings.json");
}
builder.Services.AddHttpClient("MyAPI", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    Console.WriteLine($"Configured BaseAddress for MyAPI: {apiBaseUrl}");  
});

//Tri
builder.Services.AddHttpClient<GeminiService>();

builder.Services.Configure<GeminiSettings>(
    builder.Configuration.GetSection("Gemini"));

// Register ApiSettings
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

// Register services
builder.Services.AddScoped<IHttpClientService, HttpClientService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>();

//builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddHttpClient<BlogService>();
builder.Services.AddHttpClient<AdminBlogService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7107/");
});
builder.Services.AddScoped<IAdminBlogService, AdminBlogService>(); 
builder.Services.AddScoped<ISaleProductService, SaleProductService>();



builder.Services.AddScoped<IUserService, UserService>();


builder.Services.AddScoped<IBlogCategoryServiceWeb, BlogCategoryService>();

builder.Services.AddScoped<IBlogService, BlogService>();

// Register admin services
builder.Services.AddScoped<IProductService, ProductService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.MapBlazorHub();

app.Run();
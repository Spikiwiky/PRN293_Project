using EcommerceFrontend.Web.Services;
using EcommerceFrontend.Web.Services.Admin;
using EcommerceFrontend.Web.Services.User;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Register HTTP client services
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];

builder.Services.AddHttpClient("MyAPI", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl!); 
});
builder.Services.AddScoped<IHttpClientService, HttpClientService>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>();

// Register admin services
builder.Services.AddScoped<IAdminProductService, AdminProductService>();

// Register product services
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

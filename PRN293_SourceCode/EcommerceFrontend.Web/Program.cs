using EcommerceFrontend.Web.Services;
using EcommerceFrontend.Web.Services.Admin;
using EcommerceFrontend.Web.Services.Admin.Blog;
using EcommerceFrontend.Web.Services.Blog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Register HTTP client services
builder.Services.AddHttpClient();
builder.Services.AddScoped<IHttpClientService, HttpClientService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddHttpClient<IAdminBlogService, AdminBlogService>();
builder.Services.AddHttpClient<BlogService>();
builder.Services.AddHttpClient<IAdminBlogService, AdminBlogService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7107/"); // Adjust accordingly
});


// Register admin services
builder.Services.AddScoped<IAdminProductService, AdminProductService>();

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

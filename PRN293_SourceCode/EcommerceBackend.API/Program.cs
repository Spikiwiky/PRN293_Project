using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using PdfSharp.Fonts;
using EcommerceBackend.API.Configurations;
using EcommerceBackend.API.Hubs;
using EcommerceBackend.BusinessObject.Services;
using EcommerceBackend.DataAccess.Abstract.BlogAbstract;
using EcommerceBackend.DataAccess.Repository.BlogRepository;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IBlogRepository, BlogRepository>();
builder.Services.AddScoped<BlogService>();

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


builder.Services.AddAuthorization();

//builder.Services.AddCorsPolicy(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendApp", policy =>
    {
        policy.WithOrigins("https://localhost:7107")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<SignalrHub>("/SignalrHub"); // Đăng ký đường dẫn của Hub

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseSession();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowFrontendApp");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
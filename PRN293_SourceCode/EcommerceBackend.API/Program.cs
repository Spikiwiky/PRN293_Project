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
using EcommerceBackend.BusinessObject.Services.User;
using EcommerceBackend.DataAccess.Repository.User;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

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
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddSignalR();

//builder.WebHost.UseUrls("http://0.0.0.0:80");
var app = builder.Build();

app.MapHub<SignalrHub>("/SignalrHub"); // Đăng ký đường dẫn của Hub

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseSession();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
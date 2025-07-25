namespace EcommerceBackend.API.Configurations
{
    public static class CorsConfig
    {
        public static void AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
        {
            var frontendAppUrl = configuration["Cors:FrontendAppUrl"];
            Console.WriteLine($"FrontendAppUrl loaded: {frontendAppUrl}");

            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontendApp",
                    policy =>
                    {
                        policy.WithOrigins(
                                frontendAppUrl ?? "https://localhost:7107",
                                "https://localhost:7107",
                                "http://localhost:5107",
                                "https://localhost:44321",
                                "http://localhost:44321"
                              )
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    });
            });
        }
    }
}

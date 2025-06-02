
using EcommerceFrontend.Web.Models.User;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace EcommerceFrontend.Web.Services.User;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(int userId);
    Task<bool> CreateUserAsync(UserDto newUser);
    Task<bool> UpdateUserAsync(UserDto updatedUser);
    Task<bool> DeleteUserAsync(int userId);
}

public class UserService : IUserService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<UserService> _logger;
    private readonly IConfiguration _configuration;
    private const string BaseEndpoint = "/api/Users";

    public UserService(IHttpClientFactory httpClientFactory, ILogger<UserService> logger, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _configuration = configuration;
    }

    private HttpClient CreateClient()
    {
        return _httpClientFactory.CreateClient("MyAPI");
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync(BaseEndpoint);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var users = System.Text.Json.JsonSerializer.Deserialize<List<UserDto>>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return users ?? new List<UserDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all users");
            throw;
        }
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync($"{BaseEndpoint}/{userId}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var user = System.Text.Json.JsonSerializer.Deserialize<UserDto>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to get user by id {userId}");
            throw;
        }
    }

    public async Task<bool> CreateUserAsync(UserDto newUser)
    {
        try
        {
            var client = CreateClient();
            var modifiedUser = new UserDto
            {
                UserId = newUser.UserId,
                RoleId = newUser.RoleId,
                Email = newUser.Email,
                Password = newUser.Password,  
                Phone = newUser.Phone,
                UserName = newUser.UserName,
                DateOfBirth = newUser.DateOfBirth,
                Address = newUser.Address,
                CreateDate = newUser.CreateDate == default ? DateTime.Now : newUser.CreateDate,
                Status = newUser.Status,
                IsDelete = newUser.IsDelete
                
            };
            //modifiedUser.Password = _passwordHasher.HashPassword(modifiedUser.Password); // Giả định có IPasswordHasher

            var jsonContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(modifiedUser), System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync(BaseEndpoint, jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Failed to create user. Status: {response.StatusCode}, Details: {errorContent}");
            }

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create user");
            throw;
        }
    }

    public async Task<bool> UpdateUserAsync(UserDto updatedUser)
    {
        try
        {
            var client = CreateClient();
            var jsonContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(updatedUser), System.Text.Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{BaseEndpoint}/{updatedUser.UserId}", jsonContent);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to update user {updatedUser.UserId}");
            throw;
        }
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        try
        {
            var client = CreateClient();
            var response = await client.DeleteAsync($"{BaseEndpoint}/{userId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to delete user {userId}");
            throw;
        }
    }
}

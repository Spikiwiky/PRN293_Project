using EcommerceFrontend.Web.Models.DTOs;
using EcommerceFrontend.Web.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Logging;

namespace EcommerceFrontend.Web.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IHttpClientService httpClientService, ILogger<OrderService> logger)
        {
            _httpClientService = httpClientService;
            _logger = logger;
        }

        public async Task<IEnumerable<OrderDTO>> GetOrdersByUserIdAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Calling GetOrdersByUserIdAsync for user {UserId}", userId);
                var result = await _httpClientService.GetAsync<ApiResponse<List<OrderDTO>>>("/api/Orders/user");
                _logger.LogInformation("GetOrdersByUserIdAsync result: {Result}", result != null ? "Success" : "Null");
                return result?.Data ?? new List<OrderDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOrdersByUserIdAsync for user {UserId}", userId);
                return new List<OrderDTO>();
            }
        }

        public async Task<OrderDTO?> GetOrderByIdAsync(int orderId)
        {
            try
            {
                _logger.LogInformation("Calling GetOrderByIdAsync for order {OrderId}", orderId);
                var result = await _httpClientService.GetAsync<ApiResponse<OrderDTO>>($"/api/Orders/{orderId}");
                _logger.LogInformation("GetOrderByIdAsync result: {Result}", result != null ? "Success" : "Null");
                return result?.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOrderByIdAsync for order {OrderId}", orderId);
                return null;
            }
        }

        public async Task<IEnumerable<OrderDetailDTO>> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            try
            {
                _logger.LogInformation("Calling GetOrderDetailsByOrderIdAsync for order {OrderId}", orderId);
                var result = await _httpClientService.GetAsync<ApiResponse<List<OrderDetailDTO>>>($"/api/Orders/{orderId}/details");
                _logger.LogInformation("GetOrderDetailsByOrderIdAsync result: {Result}", result != null ? "Success" : "Null");
                return result?.Data ?? new List<OrderDetailDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOrderDetailsByOrderIdAsync for order {OrderId}", orderId);
                return new List<OrderDetailDTO>();
            }
        }

        public async Task<CreateOrderResultDTO> CreateOrderFromCartAsync(int userId, string paymentMethod, string? orderNote, string? shippingAddress, int? provinceId = null, string? provinceName = null, int? districtId = null, string? districtName = null, string? wardCode = null, string? wardName = null, decimal subtotal = 0, decimal shippingFee = 0, decimal totalAmount = 0)
        {
            try
            {
                _logger.LogInformation("Calling CreateOrderFromCartAsync for user {UserId}", userId);
                var requestData = new CreateOrderRequestDTO
                {
                    PaymentMethod = paymentMethod,
                    OrderNote = orderNote,
                    ShippingAddress = shippingAddress,
                    ProvinceId = provinceId,
                    ProvinceName = provinceName,
                    DistrictId = districtId,
                    DistrictName = districtName,
                    WardCode = wardCode,
                    WardName = wardName,
                    Subtotal = subtotal,
                    ShippingFee = shippingFee,
                    TotalAmount = totalAmount
                };

                var result = await _httpClientService.PostAsync<CreateOrderResultDTO>("/api/Orders/create", requestData);
                _logger.LogInformation("CreateOrderFromCartAsync result: {Result}", result != null ? "Success" : "Null");
                return result ?? new CreateOrderResultDTO { Success = false, Message = "Không thể tạo đơn hàng" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateOrderFromCartAsync for user {UserId}", userId);
                return new CreateOrderResultDTO
                {
                    Success = false,
                    Message = "Không thể kết nối đến server"
                };
            }
        }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
    }
} 
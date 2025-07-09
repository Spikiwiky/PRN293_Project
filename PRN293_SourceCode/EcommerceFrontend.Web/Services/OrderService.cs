using EcommerceFrontend.Web.Models.DTOs;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace EcommerceFrontend.Web.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;
        public OrderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<OrderDTO>> GetAllOrdersAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<OrderDTO>>("/api/orders");
        }
        public async Task<List<OrderDetailDTO>> GetAllOrderDetailsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<OrderDetailDTO>>("/api/orders/details");
        }
    }
    public interface IOrderService
    {
        Task<List<OrderDTO>> GetAllOrdersAsync();
        Task<List<OrderDetailDTO>> GetAllOrderDetailsAsync();
    }
} 
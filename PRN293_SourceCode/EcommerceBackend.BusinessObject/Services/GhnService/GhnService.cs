using EcommerceBackend.BusinessObject.Abstract;
using EcommerceBackend.BusinessObject.dtos.GhnDto;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.Services.GhnService
{
    public class GhnService : IGhnService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GhnService> _logger;
        private readonly string _baseUrl;
        private readonly string _token;
        private readonly int _shopId;
        private readonly int _fromDistrictId;
        private readonly string _fromWardCode;

        public GhnService(HttpClient httpClient, IConfiguration configuration, ILogger<GhnService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            
            // Get GHN configuration from appsettings
            _baseUrl = _configuration["GhnSettings:BaseUrl"] ?? "https://dev-online-gateway.ghn.vn";
            _token = _configuration["GhnSettings:Token"] ?? "";
            _shopId = int.TryParse(_configuration["GhnSettings:ShopId"], out var shopId) ? shopId : 196127;
            _fromDistrictId = int.TryParse(_configuration["GhnSettings:FromDistrictId"], out var fromDistrictId) ? fromDistrictId : 2260;
            _fromWardCode = _configuration["GhnSettings:FromWardCode"] ?? "541108";
            
            if (string.IsNullOrEmpty(_token))
            {
                _logger.LogWarning("GHN Token is not configured!");
            }
        }

        public async Task<List<GhnProvinceDto>> GetProvincesAsync()
        {
            try
            {
                _logger.LogInformation("Getting provinces from GHN API...");
                _logger.LogInformation("Base URL: {BaseUrl}", _baseUrl);
                _logger.LogInformation("Token: {Token}", _token);
                
                var response = await CallGhnApiAsync<List<GhnProvinceDto>>("shiip/public-api/master-data/province", null, "GET");
                _logger.LogInformation("Provinces response: {Response}", response?.Count ?? 0);
                return response ?? new List<GhnProvinceDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting provinces from GHN");
                return new List<GhnProvinceDto>();
            }
        }

        public async Task<List<GhnDistrictDto>> GetDistrictsAsync(int provinceId)
        {
            try
            {
                var request = new { province_id = provinceId };
                var response = await CallGhnApiAsync<List<GhnDistrictDto>>("shiip/public-api/master-data/district", request, "POST");
                return response ?? new List<GhnDistrictDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting districts from GHN for province {ProvinceId}", provinceId);
                return new List<GhnDistrictDto>();
            }
        }

        public async Task<List<GhnWardDto>> GetWardsAsync(int districtId)
        {
            try
            {
                var response = await CallGhnApiAsync<List<GhnWardDto>>($"shiip/public-api/master-data/ward?district_id={districtId}", null, "GET");
                return response ?? new List<GhnWardDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting wards from GHN for district {DistrictId}", districtId);
                return new List<GhnWardDto>();
            }
        }

        public async Task<ShippingFeeResultDto> CalculateShippingFeeAsync(ShippingFeeCalculationDto request)
        {
            try
            {
                // First, get available services
                var availableServiceRequest = new
                {
                    shop_id = _shopId,
                    from_district = _fromDistrictId,
                    to_district = request.DeliveryAddress.DistrictId
                };

                var availableServices = await CallGhnApiAsync<List<GhnShippingFeeDto>>("shiip/public-api/v2/shipping-order/available-services", availableServiceRequest, "POST");
                
                if (availableServices == null || !availableServices.Any())
                {
                    return new ShippingFeeResultDto
                    {
                        Success = false,
                        Message = "Không có dịch vụ GHN cho tuyến này"
                    };
                }

                // Get the first available service
                var serviceId = availableServices.First().ServiceID;

                // Calculate fee
                var feeRequest = new
                {
                    from_district_id = _fromDistrictId,
                    from_ward_code = _fromWardCode,
                    service_id = serviceId,
                    to_district_id = request.DeliveryAddress.DistrictId,
                    to_ward_code = request.DeliveryAddress.WardCode.ToString(),
                    height = request.Height,
                    length = request.Length,
                    weight = request.Weight,
                    width = request.Width,
                    insurance_value = 0,
                    items = new[]
                    {
                        new
                        {
                            name = "Sản phẩm",
                            quantity = 1,
                            height = request.Height,
                            weight = request.Weight,
                            length = request.Length,
                            width = request.Width
                        }
                    }
                };

                var feeResponse = await CallGhnApiAsync<object>("shiip/public-api/v2/shipping-order/fee", feeRequest, "POST");
                
                if (feeResponse != null)
                {
                    // Parse the fee response
                    var feeData = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(feeResponse));
                    if (feeData.TryGetProperty("data", out var data) && data.TryGetProperty("total", out var total))
                    {
                        var totalFee = total.GetInt32();
                        return new ShippingFeeResultDto
                        {
                            Success = true,
                            Message = "Tính phí vận chuyển thành công",
                            ShippingOptions = new List<GhnShippingFeeDto>
                            {
                                new GhnShippingFeeDto
                                {
                                    ServiceID = serviceId,
                                    ServiceType = "Standard",
                                    TotalFee = totalFee,
                                    ExpectedDeliveryTime = 3
                                }
                            },
                            SelectedOption = new GhnShippingFeeDto
                            {
                                ServiceID = serviceId,
                                ServiceType = "Standard",
                                TotalFee = totalFee,
                                ExpectedDeliveryTime = 3
                            }
                        };
                    }
                }

                return new ShippingFeeResultDto
                {
                    Success = false,
                    Message = "Không thể tính phí vận chuyển cho địa chỉ này"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating shipping fee");
                return new ShippingFeeResultDto
                {
                    Success = false,
                    Message = "Lỗi khi tính phí vận chuyển"
                };
            }
        }

        public async Task<GhnCreateOrderResponseDto?> CreateShippingOrderAsync(GhnCreateOrderRequestDto request)
        {
            try
            {
                request.Token = _token;
                var response = await CallGhnApiAsync<GhnCreateOrderResponseDto>("shiip/public-api/v2/shipping-order/create", request, "POST");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating shipping order");
                return null;
            }
        }

        public async Task<string> GetTrackingInfoAsync(string orderCode)
        {
            try
            {
                var request = new { Token = _token, OrderCode = orderCode };
                var response = await CallGhnApiAsync<object>("shiip/public-api/v2/shipping-order/detail", request, "POST");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tracking info for order {OrderCode}", orderCode);
                return "Không thể lấy thông tin vận chuyển";
            }
        }

        private async Task<T?> CallGhnApiAsync<T>(string endpoint, object? request, string method = "POST")
        {
            try
            {
                var url = $"{_baseUrl}/{endpoint}";
                HttpResponseMessage response;

                _logger.LogInformation("Calling GHN API: {Method} {Url}", method, url);
                _logger.LogInformation("Request data: {Request}", request != null ? JsonSerializer.Serialize(request) : "null");

                // Add headers
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Token", _token);
                _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "EcommerceApp/1.0");
                _logger.LogInformation("Added Token header: {Token}", _token);

                if (method.ToUpper() == "GET")
                {
                    _logger.LogInformation("Making GET request to: {Url}", url);
                    response = await _httpClient.GetAsync(url);
                }
                else
                {
                    var json = request != null ? JsonSerializer.Serialize(request) : "{}";
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    _logger.LogInformation("Making POST request to: {Url} with data: {Data}", url, json);
                    response = await _httpClient.PostAsync(url, content);
                }

                _logger.LogInformation("Response status: {StatusCode}", response.StatusCode);
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("GHN API Response: {Response}", responseContent);

                if (response.IsSuccessStatusCode)
                {
                    var ghnResponse = JsonSerializer.Deserialize<GhnBaseResponseDto<T>>(responseContent);
                    if (ghnResponse?.Code == 200)
                    {
                        _logger.LogInformation("GHN API call successful");
                        return ghnResponse.Data;
                    }
                    else
                    {
                        _logger.LogWarning("GHN API returned error: {Code} - {Message}", 
                            ghnResponse?.Code, ghnResponse?.Message);
                    }
                }
                else
                {
                    _logger.LogError("GHN API request failed with status {StatusCode}: {Content}", 
                        response.StatusCode, responseContent);
                }

                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling GHN API endpoint {Endpoint}", endpoint);
                return default;
            }
        }
    }
} 
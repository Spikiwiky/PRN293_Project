using EcommerceBackend.BusinessObject.Abstract;
using EcommerceBackend.BusinessObject.dtos.GhnDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommerceBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GhnController : ControllerBase
    {
        private readonly IGhnService _ghnService;
        private readonly ILogger<GhnController> _logger;

        public GhnController(IGhnService ghnService, ILogger<GhnController> logger)
        {
            _ghnService = ghnService;
            _logger = logger;
        }

        [HttpGet("provinces")]
        public async Task<ActionResult<List<GhnProvinceDto>>> GetProvinces()
        {
            try
            {
                var provinces = await _ghnService.GetProvincesAsync();
                return Ok(new { success = true, data = provinces });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting provinces");
                return BadRequest(new { success = false, message = "Lỗi khi lấy danh sách tỉnh/thành phố" });
            }
        }

        [HttpGet("districts/{provinceId}")]
        public async Task<ActionResult<List<GhnDistrictDto>>> GetDistricts(int provinceId)
        {
            try
            {
                var districts = await _ghnService.GetDistrictsAsync(provinceId);
                return Ok(new { success = true, data = districts });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting districts for province {ProvinceId}", provinceId);
                return BadRequest(new { success = false, message = "Lỗi khi lấy danh sách quận/huyện" });
            }
        }

        [HttpGet("wards/{districtId}")]
        public async Task<ActionResult<List<GhnWardDto>>> GetWards(int districtId)
        {
            try
            {
                var wards = await _ghnService.GetWardsAsync(districtId);
                return Ok(new { success = true, data = wards });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting wards for district {DistrictId}", districtId);
                return BadRequest(new { success = false, message = "Lỗi khi lấy danh sách phường/xã" });
            }
        }

        [HttpPost("shipping-fee")]
        public async Task<ActionResult<ShippingFeeResultDto>> CalculateShippingFee([FromBody] ShippingFeeCalculationDto request)
        {
            try
            {
                var result = await _ghnService.CalculateShippingFeeAsync(request);
                return Ok(new { success = result.Success, message = result.Message, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating shipping fee");
                return BadRequest(new { success = false, message = "Lỗi khi tính phí vận chuyển" });
            }
        }

        [HttpPost("create-order")]
        public async Task<ActionResult<GhnCreateOrderResponseDto>> CreateShippingOrder([FromBody] GhnCreateOrderRequestDto request)
        {
            try
            {
                var result = await _ghnService.CreateShippingOrderAsync(request);
                if (result != null)
                {
                    return Ok(new { success = true, data = result });
                }
                return BadRequest(new { success = false, message = "Không thể tạo đơn vận chuyển" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating shipping order");
                return BadRequest(new { success = false, message = "Lỗi khi tạo đơn vận chuyển" });
            }
        }

        [HttpGet("tracking/{orderCode}")]
        public async Task<ActionResult<string>> GetTrackingInfo(string orderCode)
        {
            try
            {
                var result = await _ghnService.GetTrackingInfoAsync(orderCode);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tracking info for order {OrderCode}", orderCode);
                return BadRequest(new { success = false, message = "Lỗi khi lấy thông tin vận chuyển" });
            }
        }

        [HttpGet("test")]
        public async Task<ActionResult> TestGhnApi()
        {
            try
            {
                _logger.LogInformation("Testing GHN API connection...");
                var provinces = await _ghnService.GetProvincesAsync();
                return Ok(new { 
                    success = true, 
                    message = "GHN API test successful", 
                    provincesCount = provinces?.Count ?? 0,
                    sampleProvinces = provinces?.Take(3).Select(p => new { p.ProvinceID, p.ProvinceName })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GHN API test failed");
                return BadRequest(new { 
                    success = false, 
                    message = "GHN API test failed", 
                    error = ex.Message 
                });
            }
        }
    }
} 
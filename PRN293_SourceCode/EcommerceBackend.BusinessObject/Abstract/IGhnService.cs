using EcommerceBackend.BusinessObject.dtos.GhnDto;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EcommerceBackend.BusinessObject.Abstract
{
    public interface IGhnService
    {
        Task<List<GhnProvinceDto>> GetProvincesAsync();
        Task<List<GhnDistrictDto>> GetDistrictsAsync(int provinceId);
        Task<List<GhnWardDto>> GetWardsAsync(int districtId);
        Task<ShippingFeeResultDto> CalculateShippingFeeAsync(ShippingFeeCalculationDto request);
        Task<GhnCreateOrderResponseDto?> CreateShippingOrderAsync(GhnCreateOrderRequestDto request);
        Task<string> GetTrackingInfoAsync(string orderCode);
    }
} 
using System.Collections.Generic;

namespace EcommerceBackend.BusinessObject.dtos.GhnDto
{
    // GHN API Request DTOs
    public class GhnProvinceRequestDto
    {
        public string Token { get; set; } = string.Empty;
    }

    public class GhnDistrictRequestDto
    {
        public string Token { get; set; } = string.Empty;
        public int ProvinceId { get; set; }
    }

    public class GhnWardRequestDto
    {
        public string Token { get; set; } = string.Empty;
        public int DistrictId { get; set; }
    }

    public class GhnShippingFeeRequestDto
    {
        public string Token { get; set; } = string.Empty;
        public int ServiceTypeId { get; set; } = 2; // Standard delivery
        public int FromDistrictId { get; set; }
        public int ToDistrictId { get; set; }
        public int ToWardCode { get; set; }
        public int Height { get; set; } = 10; // cm
        public int Length { get; set; } = 20; // cm
        public int Weight { get; set; } = 500; // grams
        public int Width { get; set; } = 15; // cm
    }

    public class GhnCreateOrderRequestDto
    {
        public string Token { get; set; } = string.Empty;
        public string PaymentTypeId { get; set; } = "1"; // COD
        public string Note { get; set; } = string.Empty;
        public string RequiredNote { get; set; } = "KHONGCHOXEMHANG";
        public string ReturnPhone { get; set; } = string.Empty;
        public string ReturnAddress { get; set; } = string.Empty;
        public string ReturnDistrictId { get; set; } = string.Empty;
        public string ReturnWardCode { get; set; } = string.Empty;
        public string ClientOrderCode { get; set; } = string.Empty;
        public string ToName { get; set; } = string.Empty;
        public string ToPhone { get; set; } = string.Empty;
        public string ToAddress { get; set; } = string.Empty;
        public string ToWardCode { get; set; } = string.Empty;
        public string ToDistrictId { get; set; } = string.Empty;
        public int CodAmount { get; set; }
        public int Content { get; set; }
        public int Weight { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int InsuranceValue { get; set; }
        public int ServiceTypeId { get; set; } = 2;
        public List<GhnItemDto> Items { get; set; } = new List<GhnItemDto>();
    }

    public class GhnItemDto
    {
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int Weight { get; set; }
    }

    // GHN API Response DTOs
    public class GhnBaseResponseDto<T>
    {
        public int Code { get; set; }
        public string Message { get; set; } = string.Empty;
        public T Data { get; set; } = default!;
    }

    public class GhnProvinceDto
    {
        public int ProvinceID { get; set; }
        public string ProvinceName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

    public class GhnDistrictDto
    {
        public int DistrictID { get; set; }
        public string DistrictName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int ProvinceID { get; set; }
    }

    public class GhnWardDto
    {
        public int WardCode { get; set; }
        public string WardName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int DistrictID { get; set; }
    }

    public class GhnShippingFeeDto
    {
        public int ServiceID { get; set; }
        public string ServiceType { get; set; } = string.Empty;
        public int TotalFee { get; set; }
        public int ExpectedDeliveryTime { get; set; }
    }

    public class GhnCreateOrderResponseDto
    {
        public string OrderCode { get; set; } = string.Empty;
        public string SortCode { get; set; } = string.Empty;
        public string TransType { get; set; } = string.Empty;
        public string WardEncode { get; set; } = string.Empty;
        public string DistrictEncode { get; set; } = string.Empty;
        public string TotalFee { get; set; } = string.Empty;
        public string ExpectedDeliveryTime { get; set; } = string.Empty;
    }

    // Frontend DTOs
    public class AddressSelectionDto
    {
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int WardCode { get; set; }
        public string ProvinceName { get; set; } = string.Empty;
        public string DistrictName { get; set; } = string.Empty;
        public string WardName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }

    public class ShippingFeeCalculationDto
    {
        public int Weight { get; set; } = 500; // grams
        public int Length { get; set; } = 20; // cm
        public int Width { get; set; } = 15; // cm
        public int Height { get; set; } = 10; // cm
        public AddressSelectionDto DeliveryAddress { get; set; } = new AddressSelectionDto();
    }

    public class ShippingFeeResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<GhnShippingFeeDto> ShippingOptions { get; set; } = new List<GhnShippingFeeDto>();
        public GhnShippingFeeDto? SelectedOption { get; set; }
    }
} 
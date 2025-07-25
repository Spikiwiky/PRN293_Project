using EcommerceBackend.API.Dtos.Sale;
using EcommerceBackend.BusinessObject.Services.SaleService.OrderService;
using EcommerceBackend.BusinessObject.Services.SaleService.ProductService;
using EcommerceBackend.DataAccess.Models;
using EcommerceBackend.DataAccess.Repository.SaleRepository.OrderRepo;
using EcommerceBackend.DataAccess.Repository.SaleRepository.ProductRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceBackend.API.SaleController
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleOrderController : ControllerBase
    {
        private readonly ISaleOrderRepository _orderRepository;
        private readonly ISaleOrderService _saleService;
        private readonly IProductRepository _productRepository;

        public SaleOrderController(
            ISaleOrderRepository orderRepository,
            ISaleOrderService saleService,
            IProductRepository productRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _saleService = saleService ?? throw new ArgumentNullException(nameof(saleService));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto orderDto)
        {
            Console.WriteLine($"Received orderDto: {Newtonsoft.Json.JsonConvert.SerializeObject(orderDto)}");
            if (orderDto == null || orderDto.OrderDetails == null || !orderDto.OrderDetails.Any())
            {
                return BadRequest("Dữ liệu đơn hàng không hợp lệ hoặc không có sản phẩm.");
            }

            try
            {
                var order = new Order
                {
                    CustomerId = orderDto.CustomerId,
                    TotalQuantity = orderDto.OrderDetails.Sum(d => d.Quantity),
                    AmountDue = 0,
                    PaymentMethodId = orderDto.PaymentMethodId,
                    OrderStatusId = 1,
                    OrderNote = orderDto.OrderNote,
                    OrderDetails = new HashSet<OrderDetail>()
                };

                foreach (var detail in orderDto.OrderDetails)
                {
                    Console.WriteLine($"Processing OrderDetail: ProductId = {detail.ProductId}, Quantity = {detail.Quantity}, VariantId = {detail.VariantId}");
                    if (detail.Quantity <= 0)
                    {
                        return BadRequest($"Số lượng sản phẩm {detail.ProductId} phải lớn hơn 0.");
                    }

                    if (!detail.ProductId.HasValue)
                    {
                        return BadRequest($"ProductId trong OrderDetail là null.");
                    }

                    var product = await _productRepository.GetProductByIdAsync(detail.ProductId.Value);
                    if (product == null || product.IsDelete)
                    {
                        Console.WriteLine($"Product with ID {detail.ProductId} not found or deleted.");
                        return BadRequest($"Sản phẩm với ID {detail.ProductId} không tồn tại hoặc đã bị xóa.");
                    }
                    if (product.Status != 1)
                    {
                        Console.WriteLine($"Product with ID {detail.ProductId} is not available (Status = {product.Status}).");
                        return BadRequest($"Sản phẩm với ID {detail.ProductId} không khả dụng. (Status: {product.Status})");
                    }

                    if (!string.IsNullOrEmpty(detail.VariantId))
                    {
                        var variant = await _productRepository.GetProductVariantAsync(detail.ProductId.Value, detail.VariantId);
                        if (variant == null)
                        {
                            Console.WriteLine($"Variant with ID {detail.VariantId} not found for ProductId {detail.ProductId}.");
                            return BadRequest($"Biến thể với ID {detail.VariantId} không tồn tại cho sản phẩm {detail.ProductId}.");
                        }
                    }

                    order.OrderDetails.Add(new OrderDetail
                    {
                        OrderId = order.OrderId,
                        ProductId = detail.ProductId,
                        VariantId = detail.VariantId,
                        ProductName = product.Name,
                        Quantity = detail.Quantity,
                        Price = product.BasePrice
                    });
                    order.AmountDue += product.BasePrice * detail.Quantity;
                }

                await _saleService.CreateOrderAsync(order);
                await _orderRepository.SaveChangesAsync();

                foreach (var detail in order.OrderDetails)
                {
                    detail.OrderId = order.OrderId;
                }
                await _orderRepository.SaveChangesAsync();

                var responseDto = new OrderResponseDto
                {
                    OrderId = order.OrderId,
                    CustomerId = order.CustomerId,
                    TotalQuantity = order.TotalQuantity,
                    AmountDue = order.AmountDue,
                    PaymentMethodId = order.PaymentMethodId,
                    OrderNote = order.OrderNote,
                    OrderStatusId = order.OrderStatusId,
                    OrderDetails = order.OrderDetails?.Select(od => new OrderDetailResponseDto
                    {
                        ProductId = od.ProductId,
                        VariantId = od.VariantId,
                        Quantity = od.Quantity ?? 0,
                        Price = od.Price,
                        ProductName = od.ProductName
                    }).ToList() ?? new List<OrderDetailResponseDto>()
                };

                return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, responseDto);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"ArgumentException: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}, StackTrace: {ex.StackTrace}, InnerException: {ex.InnerException?.Message}");
                return StatusCode(500, new { Message = "Đã xảy ra lỗi server khi tạo đơn hàng.", Error = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            var orderDtos = orders.Select(o => new OrderDto
            {
                OrderId = o.OrderId,
                // Sửa lỗi Nullable object must have a value.
                // Sử dụng toán tử ?? để cung cấp giá trị mặc định 0 nếu thuộc tính là null.
                CustomerId = o.CustomerId ?? 0,          // <-- Đã sửa
                TotalQuantity = o.TotalQuantity ?? 0,    // <-- Đã sửa
                AmountDue = o.AmountDue ?? 0m,           // <-- Đã sửa (sử dụng 0m cho decimal)
                PaymentMethodId = o.PaymentMethodId ?? 0, // <-- Đã sửa
                OrderNote = o.OrderNote,
                OrderStatusId = o.OrderStatusId ?? 0,    // <-- Đã sửa
                OrderDetails = o.OrderDetails?.Select(od => new OrderDetailResponseDto
                {
                    ProductId = od.ProductId,
                    VariantId = od.VariantId,
                    Quantity = od.Quantity ?? 0,
                    Price = od.Price,
                    ProductName = od.ProductName
                }).ToList() ?? new List<OrderDetailResponseDto>() // Đảm bảo trả về list rỗng nếu OrderDetails là null
            }).ToList();
            return Ok(orderDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var responseDto = new OrderResponseDto
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                TotalQuantity = order.TotalQuantity,
                AmountDue = order.AmountDue,
                PaymentMethodId = order.PaymentMethodId,
                OrderNote = order.OrderNote,
                OrderStatusId = order.OrderStatusId,
                OrderDetails = order.OrderDetails?.Select(od => new OrderDetailResponseDto
                {
                    ProductId = od.ProductId,
                    VariantId = od.VariantId,
                    Quantity = od.Quantity ?? 0,
                    Price = od.Price,
                    ProductName = od.ProductName
                }).ToList() ?? new List<OrderDetailResponseDto>()
            };

            return Ok(responseDto);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderDto orderDto)
        {
            Console.WriteLine($"Received orderDto for update: {Newtonsoft.Json.JsonConvert.SerializeObject(orderDto)}");
            if (orderDto == null || orderDto.OrderDetails == null || !orderDto.OrderDetails.Any())
            {
                return BadRequest("Dữ liệu đơn hàng không hợp lệ hoặc không có sản phẩm.");
            }

            try
            {
                var existingOrder = await _orderRepository.GetOrderByIdAsync(id);
                if (existingOrder == null)
                {
                    return NotFound($"Đơn hàng với ID {id} không tồn tại.");
                }

                if (orderDto.CustomerId.HasValue && orderDto.CustomerId > 0)
                {
                    existingOrder.CustomerId = orderDto.CustomerId.Value;
                }

                existingOrder.PaymentMethodId = orderDto.PaymentMethodId ?? existingOrder.PaymentMethodId;

                var existingDetails = existingOrder.OrderDetails.ToList();
                var updatedDetails = new HashSet<OrderDetail>(existingDetails, new OrderDetailEqualityComparer());

                foreach (var detail in orderDto.OrderDetails)
                {
                    Console.WriteLine($"Processing OrderDetail: ProductId = {detail.ProductId}, Quantity = {detail.Quantity}, VariantId = {detail.VariantId}");
                    if (!detail.ProductId.HasValue || detail.ProductId <= 0 || detail.Quantity <= 0)
                    {
                        Console.WriteLine($"ProductId {detail.ProductId} or Quantity {detail.Quantity} is invalid, skipping update.");
                        continue;
                    }

                    var product = await _productRepository.GetProductByIdAsync(detail.ProductId.Value);
                    if (product == null || product.IsDelete)
                    {
                        Console.WriteLine($"Product with ID {detail.ProductId} not found or deleted.");
                        return BadRequest($"Sản phẩm với ID {detail.ProductId} không tồn tại hoặc đã bị xóa.");
                    }
                    if (product.Status != 1)
                    {
                        Console.WriteLine($"Product with ID {detail.ProductId} is not available (Status = {product.Status}).");
                        return BadRequest($"Sản phẩm với ID {detail.ProductId} không khả dụng. (Status: {product.Status})");
                    }

                    if (!string.IsNullOrEmpty(detail.VariantId))
                    {
                        var variant = await _productRepository.GetProductVariantAsync(detail.ProductId.Value, detail.VariantId);
                        if (variant == null)
                        {
                            Console.WriteLine($"Variant with ID {detail.VariantId} not found for ProductId {detail.ProductId}.");
                            return BadRequest($"Biến thể với ID {detail.VariantId} không tồn tại cho sản phẩm {detail.ProductId}.");
                        }
                    }

                    var existingDetail = existingDetails.FirstOrDefault(od => od.ProductId == detail.ProductId && od.VariantId == detail.VariantId);
                    if (existingDetail != null)
                    {
                        existingDetail.Quantity = detail.Quantity;
                        existingDetail.Price = product.BasePrice;
                        existingDetail.ProductName = product.Name;
                    }
                    else
                    {
                        updatedDetails.Add(new OrderDetail
                        {
                            OrderId = existingOrder.OrderId,
                            ProductId = detail.ProductId,
                            VariantId = detail.VariantId,
                            ProductName = product.Name,
                            Quantity = detail.Quantity,
                            Price = product.BasePrice
                        });
                    }
                }

                existingOrder.OrderDetails.Clear();
                foreach (var detail in updatedDetails)
                {
                    existingOrder.OrderDetails.Add(detail);
                }

                existingOrder.TotalQuantity = existingOrder.OrderDetails.Sum(d => d.Quantity ?? 0); // Handle nullable Quantity
                existingOrder.AmountDue = existingOrder.OrderDetails.Sum(d => (d.Price ?? 0m) * (d.Quantity ?? 0)); // Handle nullable Price and Quantity

                await _saleService.UpdateOrderAsync(existingOrder);
                await _orderRepository.SaveChangesAsync();

                var responseDto = new OrderResponseDto
                {
                    OrderId = existingOrder.OrderId,
                    CustomerId = existingOrder.CustomerId,
                    TotalQuantity = existingOrder.TotalQuantity,
                    AmountDue = existingOrder.AmountDue,
                    PaymentMethodId = existingOrder.PaymentMethodId,
                    OrderNote = existingOrder.OrderNote,
                    OrderStatusId = existingOrder.OrderStatusId,
                    OrderDetails = existingOrder.OrderDetails?.Select(od => new OrderDetailResponseDto
                    {
                        ProductId = od.ProductId,
                        VariantId = od.VariantId,
                        Quantity = od.Quantity ?? 0,
                        Price = od.Price,
                        ProductName = od.ProductName
                    }).ToList() ?? new List<OrderDetailResponseDto>()
                };

                return Ok(responseDto);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"ArgumentException: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}, StackTrace: {ex.StackTrace}, InnerException: {ex.InnerException?.Message}");
                return StatusCode(500, new { Message = "Đã xảy ra lỗi server khi cập nhật đơn hàng.", Error = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            Console.WriteLine($"Attempting to cancel order with ID: {id}");
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound($"Đơn hàng với ID {id} không tồn tại.");
            }

            order.OrderStatusId = 4;

            await _saleService.UpdateOrderAsync(order);
            await _orderRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            try
            {
                var details = await _saleService.GetOrderDetailsByOrderIdAsync(id);
                var response = details.Select(od => new OrderDetailResponseDto
                {
                    ProductId = od.ProductId,
                    VariantId = od.VariantId,
                    Quantity = od.Quantity ?? 0,
                    Price = od.Price,
                    ProductName = od.ProductName
                }).ToList();
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi server khi lấy chi tiết đơn hàng.", Error = ex.Message });
            }
        }

        public class OrderDetailEqualityComparer : IEqualityComparer<OrderDetail>
        {
            public bool Equals(OrderDetail x, OrderDetail y)
            {
                if (x == null || y == null)
                    return false;
                return x.ProductId == y.ProductId && x.VariantId == y.VariantId;
            }

            public int GetHashCode(OrderDetail obj)
            {
                return (obj.ProductId?.GetHashCode() ?? 0) ^ (obj.VariantId?.GetHashCode() ?? 0);
            }
        }
    }
}
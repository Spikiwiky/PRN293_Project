using EcommerceBackend.API.Dtos.Sale;
using EcommerceBackend.BusinessObject.Services.SaleService.OrderService;
using EcommerceBackend.BusinessObject.Services.SaleService.ProductService;
using EcommerceBackend.BusinessObject.Services;
using EcommerceBackend.DataAccess.Models;
using EcommerceBackend.DataAccess.Repository.SaleRepository.OrderRepo;
using EcommerceBackend.DataAccess.Repository.SaleRepository.ProductRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceBackend.BusinessObject.Services.CartService;

namespace EcommerceBackend.API.Controllers.SaleController
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleOrderController : ControllerBase
    {
        private readonly ISaleOrderRepository _orderRepository;
        private readonly ISaleOrderService _saleService;
        private readonly IProductRepository _productRepository;
        private readonly CartService _cartService;

        public SaleOrderController(
            ISaleOrderRepository orderRepository,
            ISaleOrderService saleService,
            IProductRepository productRepository,
            CartService cartService)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _saleService = saleService ?? throw new ArgumentNullException(nameof(saleService));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
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
                    ShippingAddress = orderDto.ShippingAddress,
                    OrderDetails = new HashSet<OrderDetail>()
                };

                foreach (var detail in orderDto.OrderDetails)
                {
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
                        return BadRequest($"Sản phẩm với ID {detail.ProductId} không tồn tại hoặc đã bị xóa.");
                    }
                    if (product.Status != 1)
                    {
                        return BadRequest($"Sản phẩm với ID {detail.ProductId} không khả dụng. (Status: {product.Status})");
                    }

                    if (!string.IsNullOrEmpty(detail.VariantId))
                    {
                        var variant = await _productRepository.GetProductVariantAsync(detail.ProductId.Value, detail.VariantId);
                        if (variant == null)
                        {
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
                        Price = product.BasePrice,
                        VariantAttributes = detail.VariantAttributes != null
                            ? Newtonsoft.Json.JsonConvert.SerializeObject(detail.VariantAttributes)
                            : null
                    });
                    order.AmountDue += product.BasePrice * detail.Quantity;
                }
                order.AmountDue += orderDto.ShippingFee;

                await _saleService.CreateOrderAsync(order);
                await _orderRepository.SaveChangesAsync();

                foreach (var detail in order.OrderDetails)
                {
                    detail.OrderId = order.OrderId;
                }
                await _orderRepository.SaveChangesAsync();

                //await _cartService.ClearCart(orderDto.CustomerId);

                var responseDto = new OrderResponseDto
                {
                    OrderId = order.OrderId,
                    CustomerId = order.CustomerId,
                    TotalQuantity = order.TotalQuantity,
                    AmountDue = order.AmountDue,
                    PaymentMethodId = order.PaymentMethodId,
                    OrderNote = order.OrderNote,
                    OrderStatusId = order.OrderStatusId,
                    ShippingAddress = order.ShippingAddress,
                    CreatedAt = order.CreatedAt,
                    UpdatedAt = order.UpdatedAt,
                    OrderDetails = order.OrderDetails?.Select(od => new OrderDetailResponseDto
                    {
                        ProductId = od.ProductId,
                        VariantId = od.VariantId,
                        Quantity = od.Quantity ?? 0,
                        Price = (decimal)od.Price,
                        ProductName = od.ProductName,
                        VariantAttributes = od.VariantAttributes
                    }).ToList() ?? new List<OrderDetailResponseDto>()
                };

                return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, responseDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi server khi tạo đơn hàng.", Error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _orderRepository.GetAllOrdersAsync();

                var orderDtos = orders.Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    CustomerId = o.CustomerId ?? 0,
                    TotalQuantity = o.TotalQuantity ?? 0,
                    AmountDue = o.AmountDue ?? 0.0m,
                    PaymentMethodId = o.PaymentMethodId ?? 0,
                    OrderNote = o.OrderNote,
                    OrderStatusId = o.OrderStatusId ?? 0,
                    ShippingAddress = o.ShippingAddress,
                    OrderDetails = o.OrderDetails?.Select(od => new OrderDetailResponseDto
                    {
                        ProductId = od.ProductId,
                        VariantId = od.VariantId,
                        Quantity = (int)od.Quantity,
                        Price = (decimal)od.Price,
                        ProductName = od.ProductName,
                        VariantAttributes = od.VariantAttributes
                    }).ToList()
                }).ToList();

                return Ok(orderDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi lấy danh sách đơn hàng.", Error = ex.Message });
            }
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
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                ShippingAddress = order.ShippingAddress,
                OrderDetails = order.OrderDetails?.Select(od => new OrderDetailResponseDto
                {
                    ProductId = od.ProductId,
                    VariantId = od.VariantId,
                    Quantity = od.Quantity ?? 0,
                    Price = (decimal)od.Price,
                    ProductName = od.ProductName,
                    VariantAttributes = od.VariantAttributes
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

                // Cập nhật các thông tin từ orderDto vào existingOrder
                if (orderDto.CustomerId.HasValue && orderDto.CustomerId > 0)
                {
                    existingOrder.CustomerId = orderDto.CustomerId.Value;
                }

                existingOrder.PaymentMethodId = orderDto.PaymentMethodId ?? existingOrder.PaymentMethodId;
                existingOrder.OrderNote = orderDto.OrderNote ?? existingOrder.OrderNote;
                existingOrder.ShippingAddress = orderDto.ShippingAddress ?? existingOrder.ShippingAddress;

                // Cập nhật OrderStatusId nếu có trong orderDto
                if (orderDto.OrderStatusId.HasValue)
                {
                    existingOrder.OrderStatusId = orderDto.OrderStatusId.Value;  // Cập nhật OrderStatusId
                }

                existingOrder.UpdatedAt = DateTime.UtcNow;

                var existingDetails = existingOrder.OrderDetails.ToList();
                var updatedDetails = new HashSet<OrderDetail>(existingDetails, new OrderDetailEqualityComparer());

                foreach (var detail in orderDto.OrderDetails)
                {
                    if (!detail.ProductId.HasValue || detail.ProductId <= 0 || detail.Quantity <= 0)
                    {
                        continue;
                    }

                    var product = await _productRepository.GetProductByIdAsync(detail.ProductId.Value);
                    if (product == null || product.IsDelete)
                    {
                        return BadRequest($"Sản phẩm với ID {detail.ProductId} không tồn tại hoặc đã bị xóa.");
                    }
                    if (product.Status != 1)
                    {
                        return BadRequest($"Sản phẩm với ID {detail.ProductId} không khả dụng. (Status: {product.Status})");
                    }

                    if (!string.IsNullOrEmpty(detail.VariantId))
                    {
                        var variant = await _productRepository.GetProductVariantAsync(detail.ProductId.Value, detail.VariantId);
                        if (variant == null)
                        {
                            return BadRequest($"Biến thể với ID {detail.VariantId} không tồn tại cho sản phẩm {detail.ProductId}.");
                        }
                    }

                    var existingDetail = existingDetails.FirstOrDefault(od => od.ProductId == detail.ProductId && od.VariantId == detail.VariantId);
                    if (existingDetail != null)
                    {
                        existingDetail.Quantity = detail.Quantity;
                        existingDetail.Price = product.BasePrice;
                        existingDetail.ProductName = product.Name;
                        existingDetail.VariantAttributes = detail.VariantAttributes != null
                            ? Newtonsoft.Json.JsonConvert.SerializeObject(detail.VariantAttributes)
                            : existingDetail.VariantAttributes;
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
                            Price = product.BasePrice,
                            VariantAttributes = detail.VariantAttributes != null
                                ? Newtonsoft.Json.JsonConvert.SerializeObject(detail.VariantAttributes)
                                : null
                        });
                    }
                }

                existingOrder.OrderDetails.Clear();
                foreach (var detail in updatedDetails)
                {
                    existingOrder.OrderDetails.Add(detail);
                }

                existingOrder.TotalQuantity = existingOrder.OrderDetails.Sum(d => d.Quantity);
                existingOrder.AmountDue = existingOrder.OrderDetails.Sum(d => d.Price * d.Quantity);

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
                    ShippingAddress = existingOrder.ShippingAddress,
                    CreatedAt = existingOrder.CreatedAt,
                    UpdatedAt = existingOrder.UpdatedAt,
                    OrderDetails = existingOrder.OrderDetails?.Select(od => new OrderDetailResponseDto
                    {
                        ProductId = od.ProductId,
                        VariantId = od.VariantId,
                        Quantity = od.Quantity ?? 0,
                        Price = (decimal)od.Price,
                        ProductName = od.ProductName,
                        VariantAttributes = od.VariantAttributes
                    }).ToList() ?? new List<OrderDetailResponseDto>()
                };

                return Ok(responseDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi server khi cập nhật đơn hàng.", Error = ex.Message });
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

            order.OrderStatusId = 5;

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

                var order = await _orderRepository.GetOrderByIdAsync(id);

                var response = details.Select(od => new OrderDetailResponseDto
                {
                    ProductId = od.ProductId,
                    VariantId = od.VariantId,
                    Quantity = od.Quantity ?? 0,
                    Price = (decimal)od.Price,
                    ProductName = od.ProductName,
                    VariantAttributes = od.VariantAttributes,
                    OrderStatusId = order.OrderStatusId
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
using EcommerceBackend.DataAccess.Models;
using EcommerceBackend.DataAccess.Repository.SaleRepository.ProductRepo;
using EcommerceBackend.DataAccess.Repository.SaleRepository.OrderRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.Services.SaleService.OrderService
{
    public class SaleOrderService : ISaleOrderService
    {
        private readonly IProductRepository _productRepository;
        private readonly ISaleOrderRepository _orderRepository;

        public SaleOrderService(IProductRepository productRepository, ISaleOrderRepository orderRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        // Product-related methods
        public async Task<List<Product>> GetAllProductsAsync()
        {
            return (await _productRepository.GetAllProductsAsync()).ToList();
        }

        public async Task CreateProductAsync(Product product)
        {
            await _productRepository.AddProductAsync(product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _productRepository.UpdateProductAsync(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            await _productRepository.DeleteProductAsync(id);
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllOrdersAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _orderRepository.GetOrderByIdAsync(id);
        }

        public async Task CreateOrderAsync(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            // Kiểm tra và tính toán TotalQuantity, AmountDue
            int totalQuantity = 0;
            decimal amountDue = 0;
            if (order.OrderDetails == null)
            {
                throw new ArgumentException("OrderDetails không được cung cấp.");
            }
            foreach (var detail in order.OrderDetails)
            {
                if (detail == null)
                {
                    throw new ArgumentException("Một hoặc nhiều OrderDetail là null.");
                }

                if (!detail.ProductId.HasValue)
                {
                    throw new ArgumentException($"ProductId trong OrderDetail là null.");
                }
                var product = await _productRepository.GetProductByIdAsync(detail.ProductId.Value);
                if (product == null || product.IsDelete)
                {
                    throw new ArgumentException($"Sản phẩm với ID {detail.ProductId} không tồn tại hoặc đã bị xóa.");
                }
                if (product.Status != 1) // Giả định 1 là trạng thái "Available"
                {
                    throw new ArgumentException($"Sản phẩm với ID {detail.ProductId} không khả dụng.");
                }

                // Kiểm tra VariantId nếu có
                if (!string.IsNullOrEmpty(detail.VariantId))
                {
                    var variant = await _productRepository.GetProductVariantAsync(detail.ProductId.Value, detail.VariantId);
                    if (variant == null)
                    {
                        throw new ArgumentException($"Biến thể với ID {detail.VariantId} không tồn tại cho sản phẩm {detail.ProductId}.");
                    }
                }

                // Lưu tên sản phẩm và giá vào OrderDetail
                detail.ProductName = product.Name;
                detail.Price = product.BasePrice;
                totalQuantity += detail.Quantity.Value;
                amountDue += detail.Quantity.Value * detail.Price.Value;
            }

            // Gán giá trị cho các trường nullable
            order.TotalQuantity = totalQuantity;
            order.AmountDue = amountDue;

            await _orderRepository.CreateOrderAsync(order);
        }

        public async Task UpdateOrderAsync(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            // Kiểm tra và cập nhật TotalQuantity, AmountDue
            int totalQuantity = 0;
            decimal amountDue = 0;
            if (order.OrderDetails == null)
            {
                throw new ArgumentException("OrderDetails không được cung cấp.");
            }
            foreach (var detail in order.OrderDetails)
            {
                if (detail == null)
                {
                    throw new ArgumentException("Một hoặc nhiều OrderDetail là null.");
                }

                if (!detail.ProductId.HasValue)
                {
                    throw new ArgumentException($"ProductId trong OrderDetail là null.");
                }
                var product = await _productRepository.GetProductByIdAsync(detail.ProductId.Value);
                if (product == null || product.IsDelete)
                {
                    throw new ArgumentException($"Sản phẩm với ID {detail.ProductId} không tồn tại hoặc đã bị xóa.");
                }
                if (product.Status != 1) // Giả định 1 là trạng thái "Available"
                {
                    throw new ArgumentException($"Sản phẩm với ID {detail.ProductId} không khả dụng.");
                }

                // Kiểm tra VariantId nếu có
                if (!string.IsNullOrEmpty(detail.VariantId))
                {
                    var variant = await _productRepository.GetProductVariantAsync(detail.ProductId.Value, detail.VariantId);
                    if (variant == null)
                    {
                        throw new ArgumentException($"Biến thể với ID {detail.VariantId} không tồn tại cho sản phẩm {detail.ProductId}.");
                    }
                }

                detail.ProductName = product.Name;
                detail.Price = product.BasePrice;
                totalQuantity += detail.Quantity.Value;
                amountDue += detail.Quantity.Value * detail.Price.Value;
            }

            // Gán giá trị cho các trường nullable
            order.TotalQuantity = totalQuantity;
            order.AmountDue = amountDue;

            await _orderRepository.UpdateOrderAsync(order);
        }

        //public async Task DeleteOrderAsync(int id)
        //{
        //    await _orderRepository.DeleteOrderAsync(id);
        //}
    }
}
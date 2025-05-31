//using EcommerceBackend.DataAccess.Abstract;
//using EcommerceBackend.DataAccess.Models;
//using Microsoft.EntityFrameworkCore;

//namespace EcommerceBackend.DataAccess.Repository
//{
//    public class ProductRepository : GenericRepository<Product>, IProductRepository
//    {
//        public ProductRepository(EcommerceDBContext context) : base(context)
//        {
//        }

//        public async Task<IEnumerable<Product>> GetAllProductsWithDetailsAsync()
//        {
//            return await _context.Products
//                .Include(p => p.ProductCategory)
//                .Include(p => p.ProductImages)
//                .Where(p => p.IsDelete != true)
//                .ToListAsync();
//        }

//        public async Task<Product> GetProductWithDetailsAsync(int id)
//        {
//            return await _context.Products
//                .Include(p => p.ProductCategory)
//                .Include(p => p.ProductImages)
//                .FirstOrDefaultAsync(p => p.ProductId == id && p.IsDelete != true);
//        }

//        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
//        {
//            return await _context.Products
//                .Include(p => p.ProductCategory)
//                .Include(p => p.ProductImages)
//                .Where(p => p.ProductCategoryId == categoryId && p.IsDelete != true)
//                .ToListAsync();
//        }

//        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
//        {
//            return await _context.Products
//                .Include(p => p.ProductCategory)
//                .Include(p => p.ProductImages)
//                .Where(p => p.IsDelete != true &&
//                    (p.ProductName.Contains(searchTerm) ||
//                     p.Description.Contains(searchTerm)))
//                .ToListAsync();
//        }

//        public async Task<bool> SoftDeleteAsync(int id)
//        {
//            var product = await _dbSet.FindAsync(id);
//            if (product == null) return false;

//            product.IsDelete = true;
//            return await SaveChangesAsync();
//        }
//    }
//} 
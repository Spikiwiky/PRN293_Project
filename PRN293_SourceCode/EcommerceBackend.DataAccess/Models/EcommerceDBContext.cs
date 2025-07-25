using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace EcommerceBackend.DataAccess.Models
{
    public partial class EcommerceDBContext : DbContext
    {
        public EcommerceDBContext()
        {
        }

        public EcommerceDBContext(DbContextOptions<EcommerceDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Blog> Blogs { get; set; } = null!;
        public virtual DbSet<BlogCategory> BlogCategories { get; set; } = null!;
        public virtual DbSet<Cart> Carts { get; set; } = null!;
        public virtual DbSet<CartDetail> CartDetails { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<OrderStatus> OrderStatuses { get; set; } = null!;
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductCategory> ProductCategories { get; set; } = null!;
        public virtual DbSet<ProductImage> ProductImages { get; set; } = null!;
        public virtual DbSet<ProductVariant> ProductVariants { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserRole> UserRoles { get; set; } = null!;
        public virtual DbSet<Review> Reviews { get; set; } = null!;
        public virtual DbSet<Rating> Ratings { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");

                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>(entity =>
            {
                entity.ToTable("Blog");

                entity.Property(e => e.BlogId).HasColumnName("Blog_id");

                entity.Property(e => e.BlogCategoryId).HasColumnName("Blog_category_id");

                entity.Property(e => e.BlogContent).HasColumnName("Blog_content");

                entity.Property(e => e.BlogTittle)
                    .HasMaxLength(255)
                    .HasColumnName("Blog_tittle");

                entity.HasOne(d => d.BlogCategory)
                    .WithMany(p => p.Blogs)
                    .HasForeignKey(d => d.BlogCategoryId)
                    .HasConstraintName("FK__Blog__Blog_categ__4F7CD00D");
            });

            modelBuilder.Entity<BlogCategory>(entity =>
            {
                entity.ToTable("Blog_category");

                entity.Property(e => e.BlogCategoryId).HasColumnName("Blog_category_id");

                entity.Property(e => e.BlogCategoryTitle)
                    .HasMaxLength(100)
                    .HasColumnName("Blog_category_title");

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.ToTable("Cart");

                entity.Property(e => e.CartId).HasColumnName("Cart_id");

                entity.Property(e => e.AmountDue)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("Amount_due")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.CustomerId).HasColumnName("Customer_id");

                entity.Property(e => e.TotalQuantity)
                    .HasColumnName("Total_quantity")
                    .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Cart__Customer_i__37A5467C");
            });

            modelBuilder.Entity<CartDetail>(entity =>
            {
                entity.ToTable("Cart_detail");

                entity.Property(e => e.CartDetailId).HasColumnName("Cart_detail_id");

                entity.Property(e => e.CartId).HasColumnName("Cart_id");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ProductId).HasColumnName("Product_id");

                entity.Property(e => e.ProductName)
                    .HasMaxLength(255)
                    .HasColumnName("Product_name");

                entity.Property(e => e.VariantId)
                    .HasMaxLength(50)
                    .HasColumnName("Variant_id");

                entity.Property(e => e.VariantAttributes)
                    .HasColumnName("Variant_attributes")
                    .HasDefaultValue("[]")
                    .IsRequired(false);  

                entity.HasOne(d => d.Cart)
                    .WithMany(p => p.CartDetails)
                    .HasForeignKey(d => d.CartId)
                    .HasConstraintName("FK__Cart_deta__Cart___3C69FB99");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.CartDetails)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__Cart_deta__Produ__3D5E1FD2");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.OrderId).HasColumnName("Order_id");

                entity.Property(e => e.AmountDue)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("Amount_due");

                entity.Property(e => e.CustomerId).HasColumnName("Customer_id");

                entity.Property(e => e.OrderNote)
                    .HasMaxLength(500)
                    .HasColumnName("Order_note");

                entity.Property(e => e.OrderStatusId).HasColumnName("Order_status_id");

                entity.Property(e => e.PaymentMethodId).HasColumnName("Payment_method_id");

                entity.Property(e => e.TotalQuantity).HasColumnName("Total_quantity");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Order__Customer___440B1D61");

                entity.HasOne(d => d.OrderStatus)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.OrderStatusId)
                    .HasConstraintName("FK__Order__Order_sta__45F365D3");

                entity.HasOne(d => d.PaymentMethod)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.PaymentMethodId)
                    .HasConstraintName("FK__Order__Payment_m__44FF419A");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("Order_detail");

                entity.Property(e => e.OrderDetailId).HasColumnName("Order_detail_id");

                entity.Property(e => e.OrderId).HasColumnName("Order_id");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ProductId).HasColumnName("Product_id");

                entity.Property(e => e.ProductName)
                    .HasMaxLength(255)
                    .HasColumnName("Product_name");

                entity.Property(e => e.VariantId)
                    .HasMaxLength(50)
                    .HasColumnName("Variant_id");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__Order_det__Order__48CFD27E");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__Order_det__Produ__49C3F6B7");
            });

            modelBuilder.Entity<OrderStatus>(entity =>
            {
                entity.ToTable("Order_status");

                entity.Property(e => e.OrderStatusId).HasColumnName("Order_status_id");

                entity.Property(e => e.OrderStatusTittle)
                    .HasMaxLength(100)
                    .HasColumnName("Order_status_tittle");
            });

            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.ToTable("Payment_method");

                entity.Property(e => e.PaymentMethodId).HasColumnName("Payment_method_id");

                entity.Property(e => e.PaymentMethodTittle)
                    .HasMaxLength(100)
                    .HasColumnName("Payment_method_tittle");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.Description)
                    .HasColumnName("description");

                entity.Property(e => e.ProductCategoryId)
                    .HasColumnName("product_category_id");

                entity.Property(e => e.Brand)
                    .HasMaxLength(100)
                    .HasColumnName("brand");

                entity.Property(e => e.BasePrice)
                    .HasColumnType("decimal(10,2)")
                    .HasColumnName("base_price");

                entity.Property(e => e.AvailableAttributes)
                    .HasColumnName("available_attributes");

                entity.Property(e => e.Status)
                    .HasColumnName("status");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValue(false);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime2")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime2")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne(d => d.ProductCategory)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ProductCategoryId)
                    .HasConstraintName("fk_product_category");

                entity.HasCheckConstraint("chk_available_attributes", "available_attributes IS NULL OR ISJSON(available_attributes) = 1");
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.ToTable("Product_category");

                entity.Property(e => e.ProductCategoryId).HasColumnName("Product_category_id");

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");

                entity.Property(e => e.ProductCategoryTitle)
                    .HasMaxLength(100)
                    .HasColumnName("Product_category_title");
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.ToTable("Product_image");

                entity.Property(e => e.ProductImageId).HasColumnName("Product_image_id");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(500)
                    .HasColumnName("Image_url");

                entity.Property(e => e.ProductId).HasColumnName("Product_id");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__Product_i__Produ__34C8D9D1");
            });

            modelBuilder.Entity<ProductVariant>(entity =>
            {
                entity.ToTable("variants");

                entity.Property(e => e.VariantId)
                    .HasColumnName("variant_id");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id");

                entity.Property(e => e.Attributes)
                    .HasColumnName("attributes")
                    .HasDefaultValue("{}");

                entity.Property(e => e.Variants)
                    .HasColumnName("variants")
                    .HasDefaultValue("[]");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime2")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime2")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Variants)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_product_id");

                entity.HasCheckConstraint("chk_attributes", "ISJSON(attributes) = 1");
                entity.HasCheckConstraint("chk_variants", "ISJSON(variants) = 1");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.UserId).HasColumnName("User_id");

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateOfBirth)
                    .HasColumnType("date")
                    .HasColumnName("Date_of_birth");

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");

                entity.Property(e => e.Password).HasMaxLength(255);

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.Property(e => e.RoleId).HasColumnName("Role_id");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");

                entity.Property(e => e.UserName)
                    .HasMaxLength(100)
                    .HasColumnName("User_name");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__User__Role_id__267ABA7A");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.RoleId)
                    .HasName("PK__User_rol__D80BB09309FFAC98");

                entity.ToTable("User_role");

                entity.Property(e => e.RoleId).HasColumnName("Role_id");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(100)
                    .HasColumnName("Role_name");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("reviews");

                entity.Property(e => e.ReviewId).HasColumnName("review_id");
                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Content).HasColumnName("content").HasMaxLength(1000);
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2").HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.IsVisible).HasColumnName("is_visible").HasDefaultValue(true);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.User)
                    .WithMany(u => u.Reviews)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.ToTable("ratings");

                entity.Property(e => e.RatingId).HasColumnName("rating_id");
                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Score).HasColumnName("score");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2").HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.IsVisible).HasColumnName("is_visible").HasDefaultValue(true);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Ratings)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.User)
                    .WithMany(u => u.Ratings)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

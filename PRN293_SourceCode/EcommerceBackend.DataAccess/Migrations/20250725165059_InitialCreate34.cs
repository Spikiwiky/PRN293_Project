using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceBackend.DataAccess.Migrations
{
    public partial class InitialCreate34 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VariantAttributes",
                table: "Cart_detail",
                newName: "Variant_attributes");

            migrationBuilder.AlterColumn<string>(
                name: "Product_name",
                table: "Cart_detail",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Product_id",
                table: "Cart_detail",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Cart_detail",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Cart_id",
                table: "Cart_detail",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Variant_attributes",
                table: "Cart_detail",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "[]",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Blog_category",
                columns: table => new
                {
                    Blog_category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Blog_category_title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blog_category", x => x.Blog_category_id);
                });

            migrationBuilder.CreateTable(
                name: "Order_status",
                columns: table => new
                {
                    Order_status_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Order_status_tittle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order_status", x => x.Order_status_id);
                });

            migrationBuilder.CreateTable(
                name: "Payment_method",
                columns: table => new
                {
                    Payment_method_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Payment_method_tittle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment_method", x => x.Payment_method_id);
                });

            migrationBuilder.CreateTable(
                name: "Product_category",
                columns: table => new
                {
                    Product_category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Product_category_title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product_category", x => x.Product_category_id);
                });

            migrationBuilder.CreateTable(
                name: "User_role",
                columns: table => new
                {
                    Role_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Role_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User_rol__D80BB09309FFAC98", x => x.Role_id);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    product_category_id = table.Column<int>(type: "int", nullable: false),
                    brand = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    base_price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    available_attributes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<int>(type: "int", nullable: true),
                    is_delete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.product_id);
                    table.CheckConstraint("CK_products_chk_available_attributes", "available_attributes IS NULL OR ISJSON(available_attributes) = 1");
                    table.ForeignKey(
                        name: "fk_product_category",
                        column: x => x.product_category_id,
                        principalTable: "Product_category",
                        principalColumn: "Product_category_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    User_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Role_id = table.Column<int>(type: "int", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    User_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Date_of_birth = table.Column<DateTime>(type: "date", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Create_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Status = table.Column<int>(type: "int", nullable: true, defaultValueSql: "((1))"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.User_id);
                    table.ForeignKey(
                        name: "FK__User__Role_id__267ABA7A",
                        column: x => x.Role_id,
                        principalTable: "User_role",
                        principalColumn: "Role_id");
                });

            migrationBuilder.CreateTable(
                name: "Product_image",
                columns: table => new
                {
                    Product_image_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Product_id = table.Column<int>(type: "int", nullable: true),
                    Image_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product_image", x => x.Product_image_id);
                    table.ForeignKey(
                        name: "FK__Product_i__Produ__34C8D9D1",
                        column: x => x.Product_id,
                        principalTable: "products",
                        principalColumn: "product_id");
                });

            migrationBuilder.CreateTable(
                name: "variants",
                columns: table => new
                {
                    variant_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    attributes = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "{}"),
                    variants = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "[]"),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_variants", x => x.variant_id);
                    table.CheckConstraint("CK_variants_chk_attributes", "ISJSON(attributes) = 1");
                    table.CheckConstraint("CK_variants_chk_variants", "ISJSON(variants) = 1");
                    table.ForeignKey(
                        name: "fk_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Blog",
                columns: table => new
                {
                    Blog_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Blog_category_id = table.Column<int>(type: "int", nullable: true),
                    Blog_tittle = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Blog_content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BlogSummary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BlogImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    PublishedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blog", x => x.Blog_id);
                    table.ForeignKey(
                        name: "FK__Blog__Blog_categ__4F7CD00D",
                        column: x => x.Blog_category_id,
                        principalTable: "Blog_category",
                        principalColumn: "Blog_category_id");
                    table.ForeignKey(
                        name: "FK_Blog_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "User_id");
                });

            migrationBuilder.CreateTable(
                name: "Cart",
                columns: table => new
                {
                    Cart_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Customer_id = table.Column<int>(type: "int", nullable: false),
                    Total_quantity = table.Column<int>(type: "int", nullable: true, defaultValueSql: "((0))"),
                    Amount_due = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cart", x => x.Cart_id);
                    table.ForeignKey(
                        name: "FK__Cart__Customer_i__37A5467C",
                        column: x => x.Customer_id,
                        principalTable: "User",
                        principalColumn: "User_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Order_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Customer_id = table.Column<int>(type: "int", nullable: true),
                    Total_quantity = table.Column<int>(type: "int", nullable: true),
                    Amount_due = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Payment_method_id = table.Column<int>(type: "int", nullable: true),
                    Order_status_id = table.Column<int>(type: "int", nullable: true),
                    Order_note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ShippingAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Order_id);
                    table.ForeignKey(
                        name: "FK__Order__Customer___440B1D61",
                        column: x => x.Customer_id,
                        principalTable: "User",
                        principalColumn: "User_id");
                    table.ForeignKey(
                        name: "FK__Order__Order_sta__45F365D3",
                        column: x => x.Order_status_id,
                        principalTable: "Order_status",
                        principalColumn: "Order_status_id");
                    table.ForeignKey(
                        name: "FK__Order__Payment_m__44FF419A",
                        column: x => x.Payment_method_id,
                        principalTable: "Payment_method",
                        principalColumn: "Payment_method_id");
                });

            migrationBuilder.CreateTable(
                name: "ratings",
                columns: table => new
                {
                    rating_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    score = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    is_visible = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ratings", x => x.rating_id);
                    table.ForeignKey(
                        name: "FK_ratings_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ratings_User_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "User_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reviews",
                columns: table => new
                {
                    review_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    is_visible = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reviews", x => x.review_id);
                    table.ForeignKey(
                        name: "FK_reviews_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reviews_User_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "User_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlogComment",
                columns: table => new
                {
                    CommentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlogId = table.Column<int>(type: "int", nullable: false),
                    CommenterName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommenterEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommenterWebsite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommentContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogComment", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK_BlogComment_Blog_BlogId",
                        column: x => x.BlogId,
                        principalTable: "Blog",
                        principalColumn: "Blog_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Blog_Blog_category_id",
                table: "Blog",
                column: "Blog_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_Blog_UserId",
                table: "Blog",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogComment_BlogId",
                table: "BlogComment",
                column: "BlogId");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_Customer_id",
                table: "Cart",
                column: "Customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_Customer_id",
                table: "Order",
                column: "Customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_Order_status_id",
                table: "Order",
                column: "Order_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_Payment_method_id",
                table: "Order",
                column: "Payment_method_id");

            migrationBuilder.CreateIndex(
                name: "IX_Product_image_Product_id",
                table: "Product_image",
                column: "Product_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_product_category_id",
                table: "products",
                column: "product_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_ratings_product_id",
                table: "ratings",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_ratings_user_id",
                table: "ratings",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_product_id",
                table: "reviews",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_user_id",
                table: "reviews",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_User_Role_id",
                table: "User",
                column: "Role_id");

            migrationBuilder.CreateIndex(
                name: "IX_variants_product_id",
                table: "variants",
                column: "product_id");

            migrationBuilder.AddForeignKey(
                name: "FK__Cart_deta__Cart___3C69FB99",
                table: "Cart_detail",
                column: "Cart_id",
                principalTable: "Cart",
                principalColumn: "Cart_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__Cart_deta__Produ__3D5E1FD2",
                table: "Cart_detail",
                column: "Product_id",
                principalTable: "products",
                principalColumn: "product_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__Order_det__Order__48CFD27E",
                table: "Order_detail",
                column: "Order_id",
                principalTable: "Order",
                principalColumn: "Order_id");

            migrationBuilder.AddForeignKey(
                name: "FK__Order_det__Produ__49C3F6B7",
                table: "Order_detail",
                column: "Product_id",
                principalTable: "products",
                principalColumn: "product_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Cart_deta__Cart___3C69FB99",
                table: "Cart_detail");

            migrationBuilder.DropForeignKey(
                name: "FK__Cart_deta__Produ__3D5E1FD2",
                table: "Cart_detail");

            migrationBuilder.DropForeignKey(
                name: "FK__Order_det__Order__48CFD27E",
                table: "Order_detail");

            migrationBuilder.DropForeignKey(
                name: "FK__Order_det__Produ__49C3F6B7",
                table: "Order_detail");

            migrationBuilder.DropTable(
                name: "BlogComment");

            migrationBuilder.DropTable(
                name: "Cart");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Product_image");

            migrationBuilder.DropTable(
                name: "ratings");

            migrationBuilder.DropTable(
                name: "reviews");

            migrationBuilder.DropTable(
                name: "variants");

            migrationBuilder.DropTable(
                name: "Blog");

            migrationBuilder.DropTable(
                name: "Order_status");

            migrationBuilder.DropTable(
                name: "Payment_method");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "Blog_category");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Product_category");

            migrationBuilder.DropTable(
                name: "User_role");

            migrationBuilder.RenameColumn(
                name: "Variant_attributes",
                table: "Cart_detail",
                newName: "VariantAttributes");

            migrationBuilder.AlterColumn<string>(
                name: "Product_name",
                table: "Cart_detail",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<int>(
                name: "Product_id",
                table: "Cart_detail",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Cart_detail",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Cart_id",
                table: "Cart_detail",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "VariantAttributes",
                table: "Cart_detail",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValue: "[]");
        }
    }
}

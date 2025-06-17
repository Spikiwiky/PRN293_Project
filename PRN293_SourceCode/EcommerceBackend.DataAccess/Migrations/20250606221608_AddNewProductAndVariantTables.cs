using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceBackend.DataAccess.Migrations
{
    public partial class AddNewProductAndVariantTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "Blog",
                columns: table => new
                {
                    Blog_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Blog_category_id = table.Column<int>(type: "int", nullable: true),
                    Blog_tittle = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Blog_content = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blog", x => x.Blog_id);
                    table.ForeignKey(
                        name: "FK__Blog__Blog_categ__4F7CD00D",
                        column: x => x.Blog_category_id,
                        principalTable: "Blog_category",
                        principalColumn: "Blog_category_id");
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
                    attributes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sku = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    stock = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_variants", x => x.variant_id);
                    table.CheckConstraint("CK_variants_chk_attributes", "ISJSON(attributes) = 1");
                    table.ForeignKey(
                        name: "fk_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cart",
                columns: table => new
                {
                    Cart_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Customer_id = table.Column<int>(type: "int", nullable: true),
                    Total_quantity = table.Column<int>(type: "int", nullable: true, defaultValueSql: "((0))"),
                    Amount_due = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cart", x => x.Cart_id);
                    table.ForeignKey(
                        name: "FK__Cart__Customer_i__37A5467C",
                        column: x => x.Customer_id,
                        principalTable: "User",
                        principalColumn: "User_id");
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
                    Order_note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
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
                name: "Cart_detail",
                columns: table => new
                {
                    Cart_detail_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cart_id = table.Column<int>(type: "int", nullable: true),
                    Product_id = table.Column<int>(type: "int", nullable: true),
                    Variant_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Product_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cart_detail", x => x.Cart_detail_id);
                    table.ForeignKey(
                        name: "FK__Cart_deta__Cart___3C69FB99",
                        column: x => x.Cart_id,
                        principalTable: "Cart",
                        principalColumn: "Cart_id");
                    table.ForeignKey(
                        name: "FK__Cart_deta__Produ__3D5E1FD2",
                        column: x => x.Product_id,
                        principalTable: "products",
                        principalColumn: "product_id");
                });

            migrationBuilder.CreateTable(
                name: "Order_detail",
                columns: table => new
                {
                    Order_detail_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Order_id = table.Column<int>(type: "int", nullable: true),
                    Product_id = table.Column<int>(type: "int", nullable: true),
                    Variant_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Product_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order_detail", x => x.Order_detail_id);
                    table.ForeignKey(
                        name: "FK__Order_det__Order__48CFD27E",
                        column: x => x.Order_id,
                        principalTable: "Order",
                        principalColumn: "Order_id");
                    table.ForeignKey(
                        name: "FK__Order_det__Produ__49C3F6B7",
                        column: x => x.Product_id,
                        principalTable: "products",
                        principalColumn: "product_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Blog_Blog_category_id",
                table: "Blog",
                column: "Blog_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_Customer_id",
                table: "Cart",
                column: "Customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_detail_Cart_id",
                table: "Cart_detail",
                column: "Cart_id");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_detail_Product_id",
                table: "Cart_detail",
                column: "Product_id");

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
                name: "IX_Order_detail_Order_id",
                table: "Order_detail",
                column: "Order_id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_detail_Product_id",
                table: "Order_detail",
                column: "Product_id");

            migrationBuilder.CreateIndex(
                name: "IX_Product_image_Product_id",
                table: "Product_image",
                column: "Product_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_product_category_id",
                table: "products",
                column: "product_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_User_Role_id",
                table: "User",
                column: "Role_id");

            migrationBuilder.CreateIndex(
                name: "IX_variants_product_id",
                table: "variants",
                column: "product_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Blog");

            migrationBuilder.DropTable(
                name: "Cart_detail");

            migrationBuilder.DropTable(
                name: "Order_detail");

            migrationBuilder.DropTable(
                name: "Product_image");

            migrationBuilder.DropTable(
                name: "variants");

            migrationBuilder.DropTable(
                name: "Blog_category");

            migrationBuilder.DropTable(
                name: "Cart");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Order_status");

            migrationBuilder.DropTable(
                name: "Payment_method");

            migrationBuilder.DropTable(
                name: "Product_category");

            migrationBuilder.DropTable(
                name: "User_role");
        }
    }
}

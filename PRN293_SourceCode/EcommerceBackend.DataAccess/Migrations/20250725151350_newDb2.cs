using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceBackend.DataAccess.Migrations
{
    public partial class newDb2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Cart_deta__Cart___3C69FB99",
                table: "Cart_detail");

            migrationBuilder.DropForeignKey(
                name: "FK__Cart_deta__Produ__3D5E1FD2",
                table: "Cart_detail");

            migrationBuilder.AlterColumn<string>(
                name: "Variant_attributes",
                table: "Cart_detail",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "[]",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_detail_Cart_Cart_id",
                table: "Cart_detail",
                column: "Cart_id",
                principalTable: "Cart",
                principalColumn: "Cart_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_detail_products_Product_id",
                table: "Cart_detail",
                column: "Product_id",
                principalTable: "products",
                principalColumn: "product_id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_detail_Cart_Cart_id",
                table: "Cart_detail");

            migrationBuilder.DropForeignKey(
                name: "FK_Cart_detail_products_Product_id",
                table: "Cart_detail");

            migrationBuilder.AlterColumn<string>(
                name: "Variant_attributes",
                table: "Cart_detail",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValue: "[]");

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
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceBackend.DataAccess.Migrations
{
    public partial class UpdateVariantIdToInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First, update any non-numeric VariantId values to NULL
            migrationBuilder.Sql("UPDATE Cart_detail SET Variant_id = NULL WHERE Variant_id IS NOT NULL AND ISNUMERIC(Variant_id) = 0");
            migrationBuilder.Sql("UPDATE Order_detail SET Variant_id = NULL WHERE Variant_id IS NOT NULL AND ISNUMERIC(Variant_id) = 0");
            
            // Convert empty strings to NULL
            migrationBuilder.Sql("UPDATE Cart_detail SET Variant_id = NULL WHERE Variant_id = ''");
            migrationBuilder.Sql("UPDATE Order_detail SET Variant_id = NULL WHERE Variant_id = ''");
            
            // Change Cart_detail.Variant_id from string to int
            migrationBuilder.AlterColumn<int>(
                name: "Variant_id",
                table: "Cart_detail",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);
            
            // Change Order_detail.Variant_id from string to int
            migrationBuilder.AlterColumn<int>(
                name: "Variant_id",
                table: "Order_detail",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert Cart_detail.Variant_id back to string
            migrationBuilder.AlterColumn<string>(
                name: "Variant_id",
                table: "Cart_detail",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
            
            // Revert Order_detail.Variant_id back to string
            migrationBuilder.AlterColumn<string>(
                name: "Variant_id",
                table: "Order_detail",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
} 
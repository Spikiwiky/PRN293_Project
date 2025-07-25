using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceBackend.DataAccess.Migrations
{
    public partial class FixVariantIdFinal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Fix Variant_id column in Cart_detail table
            migrationBuilder.Sql("UPDATE Cart_detail SET Variant_id = NULL WHERE Variant_id IS NOT NULL AND ISNUMERIC(Variant_id) = 0");
            migrationBuilder.Sql("UPDATE Cart_detail SET Variant_id = NULL WHERE Variant_id = ''");
            
            migrationBuilder.AlterColumn<int>(
                name: "Variant_id",
                table: "Cart_detail",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Variant_id",
                table: "Cart_detail",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
} 
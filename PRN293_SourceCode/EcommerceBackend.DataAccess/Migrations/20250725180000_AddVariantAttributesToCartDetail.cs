using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceBackend.DataAccess.Migrations
{
    public partial class AddVariantAttributesToCartDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add Variant_attributes column if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = 'Cart_detail' AND COLUMN_NAME = 'Variant_attributes')
                BEGIN
                    ALTER TABLE Cart_detail ADD Variant_attributes nvarchar(max) NULL DEFAULT '[]'
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Variant_attributes",
                table: "Cart_detail");
        }
    }
} 
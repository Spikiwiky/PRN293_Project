using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceBackend.DataAccess.Migrations
{
    public partial class newDb4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Variant_attributes",
                table: "Cart_detail",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "[]",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "[]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Variant_attributes",
                table: "Cart_detail",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValue: "[]");
        }
    }
}

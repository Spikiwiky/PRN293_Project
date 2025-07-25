using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceBackend.DataAccess.Migrations
{
    public partial class newDb6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Thêm dòng này để cập nhật dữ liệu NULL hiện có thành '[]'
            // Điều này cực kỳ quan trọng để tránh lỗi NOT NULL khi thay đổi cột.
            migrationBuilder.Sql("UPDATE Cart_detail SET Variant_attributes = '[]' WHERE Variant_attributes IS NULL;");

            migrationBuilder.AlterColumn<string>(
                name: "Variant_attributes",
                table: "Cart_detail",
                type: "nvarchar(max)", // hoặc kiểu dữ liệu cụ thể
                nullable: true, // Entity Framework sẽ tự động đặt là true
                defaultValue: "[]", // Giá trị mặc định
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: false); // Đây là trạng thái cũ nếu trước đó nó là NOT NULL
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Đảo ngược lại các thay đổi trong phương thức Down
            // Nếu bạn có dữ liệu '[]' và muốn chuyển lại về NULL, bạn cần SQL ở đây
            migrationBuilder.AlterColumn<string>(
                name: "Variant_attributes",
                table: "Cart_detail",
                type: "nvarchar(max)",
                nullable: false, // Trở lại NOT NULL nếu bạn muốn
                defaultValue: null, // Hoặc giá trị mặc định ban đầu nếu có
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValue: "[]");
        }
    }
}

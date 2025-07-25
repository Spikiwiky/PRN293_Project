using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceBackend.DataAccess.Migrations
{
    public partial class InitialCreate30 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Blog");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Blog",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blog_UserId",
                table: "Blog",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Blog_User_UserId",
                table: "Blog",
                column: "UserId",
                principalTable: "User",
                principalColumn: "User_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blog_User_UserId",
                table: "Blog");

            migrationBuilder.DropIndex(
                name: "IX_Blog_UserId",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Blog");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Blog",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

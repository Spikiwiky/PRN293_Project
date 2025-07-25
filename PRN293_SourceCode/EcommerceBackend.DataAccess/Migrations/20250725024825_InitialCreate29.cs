using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceBackend.DataAccess.Migrations
{
    public partial class InitialCreate29 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "IX_BlogComment_BlogId",
                table: "BlogComment",
                column: "BlogId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogComment");
        }
    }
}

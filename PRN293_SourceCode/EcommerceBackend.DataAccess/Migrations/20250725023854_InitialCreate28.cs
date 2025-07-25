using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceBackend.DataAccess.Migrations
{
    public partial class InitialCreate28 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Order",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Order",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Blog",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BlogImage",
                table: "Blog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BlogSummary",
                table: "Blog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Blog",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Blog",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishedDate",
                table: "Blog",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Blog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Blog",
                type: "datetime2",
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
                principalColumn: "UserId");
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
                name: "CreatedAt",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ShippingAddress",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "BlogImage",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "BlogSummary",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "PublishedDate",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Blog");
        }
    }
}

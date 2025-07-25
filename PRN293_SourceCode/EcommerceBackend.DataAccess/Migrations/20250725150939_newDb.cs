using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceBackend.DataAccess.Migrations
{
    public partial class newDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Cart__Customer_i__37A5467C",
                table: "Cart");

            migrationBuilder.DropForeignKey(
                name: "FK__Cart_deta__Cart___3C69FB99",
                table: "Cart_detail");

            migrationBuilder.DropForeignKey(
                name: "FK__Cart_deta__Produ__3D5E1FD2",
                table: "Cart_detail");

            migrationBuilder.RenameColumn(
                name: "VariantAttributes",
                table: "Cart_detail",
                newName: "Variant_attributes");

            migrationBuilder.AlterColumn<string>(
                name: "Variant_id",
                table: "Cart_detail",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Product_name",
                table: "Cart_detail",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Product_id",
                table: "Cart_detail",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Cart_detail",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Cart_id",
                table: "Cart_detail",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Variant_attributes",
                table: "Cart_detail",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Customer_id",
                table: "Cart",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount_due",
                table: "Cart",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true,
                defaultValueSql: "((0))",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true,
                oldDefaultValueSql: "((0))");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "Cart",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "Cart",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK__Cart__Customer_i__37A5467C",
                table: "Cart",
                column: "Customer_id",
                principalTable: "User",
                principalColumn: "User_id",
                onDelete: ReferentialAction.Cascade);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Cart__Customer_i__37A5467C",
                table: "Cart");

            migrationBuilder.DropForeignKey(
                name: "FK__Cart_deta__Cart___3C69FB99",
                table: "Cart_detail");

            migrationBuilder.DropForeignKey(
                name: "FK__Cart_deta__Produ__3D5E1FD2",
                table: "Cart_detail");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "Cart");

            migrationBuilder.RenameColumn(
                name: "Variant_attributes",
                table: "Cart_detail",
                newName: "VariantAttributes");

            migrationBuilder.AlterColumn<string>(
                name: "Variant_id",
                table: "Cart_detail",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Product_name",
                table: "Cart_detail",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<int>(
                name: "Product_id",
                table: "Cart_detail",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Cart_detail",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Cart_id",
                table: "Cart_detail",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "VariantAttributes",
                table: "Cart_detail",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Customer_id",
                table: "Cart",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount_due",
                table: "Cart",
                type: "decimal(18,2)",
                nullable: true,
                defaultValueSql: "((0))",
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldNullable: true,
                oldDefaultValueSql: "((0))");

            migrationBuilder.AddForeignKey(
                name: "FK__Cart__Customer_i__37A5467C",
                table: "Cart",
                column: "Customer_id",
                principalTable: "User",
                principalColumn: "User_id");

            migrationBuilder.AddForeignKey(
                name: "FK__Cart_deta__Cart___3C69FB99",
                table: "Cart_detail",
                column: "Cart_id",
                principalTable: "Cart",
                principalColumn: "Cart_id");

            migrationBuilder.AddForeignKey(
                name: "FK__Cart_deta__Produ__3D5E1FD2",
                table: "Cart_detail",
                column: "Product_id",
                principalTable: "products",
                principalColumn: "product_id");
        }
    }
}

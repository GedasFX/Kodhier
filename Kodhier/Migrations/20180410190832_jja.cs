using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Kodhier.Migrations
{
    public partial class jja : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PizzaPriceCategoryId",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Orders",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PizzaPriceCategoryId",
                table: "Orders",
                column: "PizzaPriceCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_PizzaPriceCategories_PizzaPriceCategoryId",
                table: "Orders",
                column: "PizzaPriceCategoryId",
                principalTable: "PizzaPriceCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_PizzaPriceCategories_PizzaPriceCategoryId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PizzaPriceCategoryId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PizzaPriceCategoryId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Orders");
        }
    }
}

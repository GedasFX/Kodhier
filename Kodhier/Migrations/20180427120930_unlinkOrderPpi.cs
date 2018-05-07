using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Kodhier.Migrations
{
    public partial class unlinkOrderPpi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PizzaPriceCategoryId",
                table: "Orders",
                nullable: true);

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
    }
}

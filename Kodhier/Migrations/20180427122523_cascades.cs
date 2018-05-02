using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Kodhier.Migrations
{
    public partial class cascades : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pizzas_PizzaPriceCategories_PriceCategoryId",
                table: "Pizzas");

            migrationBuilder.AddForeignKey(
                name: "FK_Pizzas_PizzaPriceCategories_PriceCategoryId",
                table: "Pizzas",
                column: "PriceCategoryId",
                principalTable: "PizzaPriceCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pizzas_PizzaPriceCategories_PriceCategoryId",
                table: "Pizzas");

            migrationBuilder.AddForeignKey(
                name: "FK_Pizzas_PizzaPriceCategories_PriceCategoryId",
                table: "Pizzas",
                column: "PriceCategoryId",
                principalTable: "PizzaPriceCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

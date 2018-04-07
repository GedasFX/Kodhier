using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Kodhier.Migrations
{
    public partial class PizzaPriceCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Pizzas");

            migrationBuilder.DropColumn(
                name: "ValidSizes",
                table: "Pizzas");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "PriceCategoryId",
                table: "Pizzas",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PizzaPriceCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PizzaPriceCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PizzaPriceInfo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Price = table.Column<decimal>(nullable: false),
                    PriceCategoryId = table.Column<int>(nullable: false),
                    Size = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PizzaPriceInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PizzaPriceInfo_PizzaPriceCategories_PriceCategoryId",
                        column: x => x.PriceCategoryId,
                        principalTable: "PizzaPriceCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pizzas_PriceCategoryId",
                table: "Pizzas",
                column: "PriceCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PizzaPriceInfo_PriceCategoryId",
                table: "PizzaPriceInfo",
                column: "PriceCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pizzas_PizzaPriceCategories_PriceCategoryId",
                table: "Pizzas",
                column: "PriceCategoryId",
                principalTable: "PizzaPriceCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pizzas_PizzaPriceCategories_PriceCategoryId",
                table: "Pizzas");

            migrationBuilder.DropTable(
                name: "PizzaPriceInfo");

            migrationBuilder.DropTable(
                name: "PizzaPriceCategories");

            migrationBuilder.DropIndex(
                name: "IX_Pizzas_PriceCategoryId",
                table: "Pizzas");

            migrationBuilder.DropColumn(
                name: "PriceCategoryId",
                table: "Pizzas");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Pizzas",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ValidSizes",
                table: "Pizzas",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Size",
                table: "Orders",
                nullable: false,
                defaultValue: 0);
        }
    }
}

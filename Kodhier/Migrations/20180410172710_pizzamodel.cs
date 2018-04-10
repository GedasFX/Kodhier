using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Kodhier.Migrations
{
    public partial class pizzamodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pizzas_AspNetUsers_CreatorId",
                table: "Pizzas");

            migrationBuilder.DropIndex(
                name: "IX_Pizzas_CreatorId",
                table: "Pizzas");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Pizzas");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "Pizzas",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pizzas_CreatorId",
                table: "Pizzas",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pizzas_AspNetUsers_CreatorId",
                table: "Pizzas",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

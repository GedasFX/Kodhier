using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Kodhier.Migrations
{
    public partial class deliveree : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DelivereeId",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryColor",
                table: "Orders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DelivereeId",
                table: "Orders",
                column: "DelivereeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_DelivereeId",
                table: "Orders",
                column: "DelivereeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_DelivereeId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_DelivereeId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DelivereeId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryColor",
                table: "Orders");
        }
    }
}

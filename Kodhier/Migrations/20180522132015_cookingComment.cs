using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Kodhier.Migrations
{
    public partial class cookingComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Comment",
                table: "Orders",
                newName: "CookingComment");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryComment",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryComment",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "CookingComment",
                table: "Orders",
                newName: "Comment");
        }
    }
}

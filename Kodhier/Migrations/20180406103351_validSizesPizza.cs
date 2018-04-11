using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Kodhier.Migrations
{
    public partial class validSizesPizza : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ValidValues",
                table: "Pizzas",
                newName: "ValidSizes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ValidSizes",
                table: "Pizzas",
                newName: "ValidValues");
        }
    }
}

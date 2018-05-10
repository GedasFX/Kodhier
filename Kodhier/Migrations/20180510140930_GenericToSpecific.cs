using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Kodhier.Migrations
{
    public partial class GenericToSpecific : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Pizzas",
                newName: "NameLt");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Pizzas",
                newName: "DescriptionLt");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "Pizzas",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "Pizzas");

            migrationBuilder.RenameColumn(
                name: "NameLt",
                table: "Pizzas",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "DescriptionLt",
                table: "Pizzas",
                newName: "Description");
        }
    }
}

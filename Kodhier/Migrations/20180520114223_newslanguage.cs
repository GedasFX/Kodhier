using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Kodhier.Migrations
{
    public partial class newslanguage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "News",
                newName: "TitleLt");

            migrationBuilder.RenameColumn(
                name: "Caption",
                table: "News",
                newName: "CaptionLt");

            migrationBuilder.AddColumn<string>(
                name: "CaptionEn",
                table: "News",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleEn",
                table: "News",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaptionEn",
                table: "News");

            migrationBuilder.DropColumn(
                name: "TitleEn",
                table: "News");

            migrationBuilder.RenameColumn(
                name: "TitleLt",
                table: "News",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "CaptionLt",
                table: "News",
                newName: "Caption");
        }
    }
}

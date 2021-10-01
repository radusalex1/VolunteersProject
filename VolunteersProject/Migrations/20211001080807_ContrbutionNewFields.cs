using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VolunteersProject.Migrations
{
    public partial class ContrbutionNewFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FinishDate",
                table: "Contributions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Contributions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinishDate",
                table: "Contributions");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Contributions");
        }
    }
}

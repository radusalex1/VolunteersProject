using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VolunteersProject.Migrations
{
    public partial class Add_ImageProfile_To_VolunteerModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ImageProfileByteArray",
                table: "Volunteers",
                type: "varbinary(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageProfileByteArray",
                table: "Volunteers");
        }
    }
}

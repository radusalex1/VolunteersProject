using Microsoft.EntityFrameworkCore.Migrations;

namespace VolunteersProject.Migrations
{
    public partial class UserIdFieldVolunteer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "Volunteers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Volunteers");
        }
    }
}

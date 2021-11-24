using Microsoft.EntityFrameworkCore.Migrations;

namespace VolunteersProject.Migrations
{
    public partial class VolunteerDeadlineConfirmation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateUntilConfirmation",
                table: "Contributions",
                newName: "VolunteerDeadlineConfirmation");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VolunteerDeadlineConfirmation",
                table: "Contributions",
                newName: "DateUntilConfirmation");
        }
    }
}

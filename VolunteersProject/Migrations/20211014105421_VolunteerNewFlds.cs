using Microsoft.EntityFrameworkCore.Migrations;

namespace VolunteersProject.Migrations
{
    public partial class VolunteerNewFlds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescriptionContributionToHub",
                table: "Volunteers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FaceBookProfile",
                table: "Volunteers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstagramProfile",
                table: "Volunteers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionContributionToHub",
                table: "Volunteers");

            migrationBuilder.DropColumn(
                name: "FaceBookProfile",
                table: "Volunteers");

            migrationBuilder.DropColumn(
                name: "InstagramProfile",
                table: "Volunteers");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace VolunteersProject.Migrations
{
    public partial class descriptionFieldToContribution : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Contributions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Contributions");
        }
    }
}

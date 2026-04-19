using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlindProjectApproval.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProjectForStudentModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Projects",
                newName: "TechStack");

            migrationBuilder.AddColumn<string>(
                name: "Abstract",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResearchArea",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Abstract",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ResearchArea",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "TechStack",
                table: "Projects",
                newName: "Description");
        }
    }
}

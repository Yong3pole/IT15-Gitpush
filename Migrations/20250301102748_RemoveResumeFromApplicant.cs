using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IT15_TripoleMedelTijol.Migrations
{
    /// <inheritdoc />
    public partial class RemoveResumeFromApplicant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResumePath",
                table: "Applicants");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResumePath",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

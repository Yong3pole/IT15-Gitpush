using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IT15_TripoleMedelTijol.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmployeeViewUpdateJobTitleFilled : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmployeeId",
                table: "JobTitles",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFilled",
                table: "JobTitles",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobTitles_EmployeeId",
                table: "JobTitles",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobTitles_Employees_EmployeeId",
                table: "JobTitles",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobTitles_Employees_EmployeeId",
                table: "JobTitles");

            migrationBuilder.DropIndex(
                name: "IX_JobTitles_EmployeeId",
                table: "JobTitles");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "JobTitles");

            migrationBuilder.DropColumn(
                name: "IsFilled",
                table: "JobTitles");
        }
    }
}

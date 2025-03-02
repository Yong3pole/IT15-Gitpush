using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IT15_TripoleMedelTijol.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmployeesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WorkPhoneNumber",
                table: "Employees",
                newName: "ResumePath");

            migrationBuilder.RenameColumn(
                name: "WorkEmail",
                table: "Employees",
                newName: "PhoneNumber");

            migrationBuilder.AddColumn<int>(
                name: "ApplicantID",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ApplicantID",
                table: "Employees",
                column: "ApplicantID");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Applicants_ApplicantID",
                table: "Employees",
                column: "ApplicantID",
                principalTable: "Applicants",
                principalColumn: "ApplicantID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Applicants_ApplicantID",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_ApplicantID",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ApplicantID",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "ResumePath",
                table: "Employees",
                newName: "WorkPhoneNumber");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Employees",
                newName: "WorkEmail");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IT15_TripoleMedelTijol.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGeneratePayrollVersion2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LateMinutes",
                table: "EmployeeAttendances");

            migrationBuilder.DropColumn(
                name: "OvertimeMinutes",
                table: "EmployeeAttendances");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LateMinutes",
                table: "EmployeeAttendances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OvertimeMinutes",
                table: "EmployeeAttendances",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

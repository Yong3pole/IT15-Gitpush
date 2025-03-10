using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IT15_TripoleMedelTijol.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedingForLeaveTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "LeaveTypes",
                columns: new[] { "LeaveTypeId", "DefaultDays", "IsPaid", "Name" },
                values: new object[,]
                {
                    { 1, 10, true, "Sick Leave" },
                    { 2, 15, true, "Vacation Leave" },
                    { 3, 60, true, "Maternity Leave" },
                    { 4, 7, true, "Paternity Leave" },
                    { 5, 0, false, "Unpaid Leave" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LeaveTypes",
                keyColumn: "LeaveTypeId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "LeaveTypes",
                keyColumn: "LeaveTypeId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "LeaveTypes",
                keyColumn: "LeaveTypeId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "LeaveTypes",
                keyColumn: "LeaveTypeId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "LeaveTypes",
                keyColumn: "LeaveTypeId",
                keyValue: 5);
        }
    }
}

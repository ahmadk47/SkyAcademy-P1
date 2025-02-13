using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class ApprovedDeclinedModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VacationRequests_Employees_ApprovedBy",
                table: "VacationRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_VacationRequests_Employees_DeclinedBy",
                table: "VacationRequests");

            migrationBuilder.DropIndex(
                name: "IX_VacationRequests_ApprovedBy",
                table: "VacationRequests");

            migrationBuilder.DropIndex(
                name: "IX_VacationRequests_DeclinedBy",
                table: "VacationRequests");

            migrationBuilder.AlterColumn<string>(
                name: "DeclinedBy",
                table: "VacationRequests",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(6)",
                oldMaxLength: 6);

            migrationBuilder.AlterColumn<string>(
                name: "ApprovedBy",
                table: "VacationRequests",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(6)",
                oldMaxLength: 6);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DeclinedBy",
                table: "VacationRequests",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(6)",
                oldMaxLength: 6,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApprovedBy",
                table: "VacationRequests",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(6)",
                oldMaxLength: 6,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VacationRequests_ApprovedBy",
                table: "VacationRequests",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_VacationRequests_DeclinedBy",
                table: "VacationRequests",
                column: "DeclinedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_VacationRequests_Employees_ApprovedBy",
                table: "VacationRequests",
                column: "ApprovedBy",
                principalTable: "Employees",
                principalColumn: "EmployeeNumber",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VacationRequests_Employees_DeclinedBy",
                table: "VacationRequests",
                column: "DeclinedBy",
                principalTable: "Employees",
                principalColumn: "EmployeeNumber",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

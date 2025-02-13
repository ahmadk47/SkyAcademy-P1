using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EmployeeManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DepartmentId);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    PositionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PositionName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.PositionId);
                });

            migrationBuilder.CreateTable(
                name: "RequestStates",
                columns: table => new
                {
                    StateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StateName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestStates", x => x.StateId);
                });

            migrationBuilder.CreateTable(
                name: "VacationTypes",
                columns: table => new
                {
                    VacationTypeCode = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    VacationTypeName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacationTypes", x => x.VacationTypeCode);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeNumber = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    PositionId = table.Column<int>(type: "int", nullable: false),
                    GenderCode = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    ReportedToEmployeeNumber = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    VacationDaysLeft = table.Column<int>(type: "int", nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeNumber);
                    table.ForeignKey(
                        name: "FK_Employees_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Employees_Employees_ReportedToEmployeeNumber",
                        column: x => x.ReportedToEmployeeNumber,
                        principalTable: "Employees",
                        principalColumn: "EmployeeNumber",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "VacationRequests",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EmployeeNumber = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    VacationTypeCode = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalVacationDays = table.Column<int>(type: "int", nullable: false),
                    RequestStateId = table.Column<int>(type: "int", nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    DeclinedBy = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacationRequests", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_VacationRequests_Employees_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "Employees",
                        principalColumn: "EmployeeNumber",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_VacationRequests_Employees_DeclinedBy",
                        column: x => x.DeclinedBy,
                        principalTable: "Employees",
                        principalColumn: "EmployeeNumber",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_VacationRequests_Employees_EmployeeNumber",
                        column: x => x.EmployeeNumber,
                        principalTable: "Employees",
                        principalColumn: "EmployeeNumber",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_VacationRequests_RequestStates_RequestStateId",
                        column: x => x.RequestStateId,
                        principalTable: "RequestStates",
                        principalColumn: "StateId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_VacationRequests_VacationTypes_VacationTypeCode",
                        column: x => x.VacationTypeCode,
                        principalTable: "VacationTypes",
                        principalColumn: "VacationTypeCode",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.InsertData(
                table: "RequestStates",
                columns: new[] { "StateId", "StateName" },
                values: new object[,]
                {
                    { 1, "Submitted" },
                    { 2, "Approved" },
                    { 3, "Declined" }
                });

            migrationBuilder.InsertData(
                table: "VacationTypes",
                columns: new[] { "VacationTypeCode", "VacationTypeName" },
                values: new object[,]
                {
                    { "A", "Annual" },
                    { "B", "Business Trip" },
                    { "O", "Day Off" },
                    { "S", "Sick" },
                    { "U", "Unpaid" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PositionId",
                table: "Employees",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ReportedToEmployeeNumber",
                table: "Employees",
                column: "ReportedToEmployeeNumber");

            migrationBuilder.CreateIndex(
                name: "IX_VacationRequests_ApprovedBy",
                table: "VacationRequests",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_VacationRequests_DeclinedBy",
                table: "VacationRequests",
                column: "DeclinedBy");

            migrationBuilder.CreateIndex(
                name: "IX_VacationRequests_EmployeeNumber",
                table: "VacationRequests",
                column: "EmployeeNumber");

            migrationBuilder.CreateIndex(
                name: "IX_VacationRequests_RequestStateId",
                table: "VacationRequests",
                column: "RequestStateId");

            migrationBuilder.CreateIndex(
                name: "IX_VacationRequests_VacationTypeCode",
                table: "VacationRequests",
                column: "VacationTypeCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VacationRequests");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "RequestStates");

            migrationBuilder.DropTable(
                name: "VacationTypes");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Positions");
        }
    }
}

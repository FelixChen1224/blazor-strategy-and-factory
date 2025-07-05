using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace blazor_strategy_and_factory.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BUDGET_RECORDS",
                columns: table => new
                {
                    BUDGET_ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DEPARTMENT = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    BUDGET_YEAR = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    BUDGET_MONTH = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    PLANNED_AMOUNT = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: false),
                    ACTUAL_AMOUNT = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: false),
                    VARIANCE = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: false),
                    REGION = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    CATEGORY = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    STATUS = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    CREATED_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UPDATED_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BUDGET_RECORDS", x => x.BUDGET_ID);
                });

            migrationBuilder.CreateTable(
                name: "COMPANY_ANNOUNCEMENTS",
                columns: table => new
                {
                    ANNOUNCEMENT_ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    TITLE = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    CONTENT = table.Column<string>(type: "NCLOB", maxLength: 4000, nullable: false),
                    ANNOUNCEMENT_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ANNOUNCEMENT_TYPE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    REGION = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    IMPORTANCE_LEVEL = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    STATUS = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    CREATED_BY = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    CREATED_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UPDATED_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COMPANY_ANNOUNCEMENTS", x => x.ANNOUNCEMENT_ID);
                });

            migrationBuilder.CreateTable(
                name: "EMPLOYEES",
                columns: table => new
                {
                    EMPLOYEE_ID = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    EMPLOYEE_NAME = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    DEPARTMENT = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    REGION = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    POSITION = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    HIRE_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    SALARY = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: false),
                    CREATED_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UPDATED_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPLOYEES", x => x.EMPLOYEE_ID);
                });

            migrationBuilder.CreateTable(
                name: "FINANCIAL_RECORDS",
                columns: table => new
                {
                    RECORD_ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    EMPLOYEE_ID = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    RECORD_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    AMOUNT = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: false),
                    TRANSACTION_TYPE = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    DESCRIPTION = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    REGION = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    CATEGORY = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    STATUS = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    CREATED_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UPDATED_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FINANCIAL_RECORDS", x => x.RECORD_ID);
                });

            migrationBuilder.CreateTable(
                name: "MARKET_DATA",
                columns: table => new
                {
                    DATA_ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    SYMBOL = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    MARKET_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    OPEN_PRICE = table.Column<decimal>(type: "DECIMAL(18,4)", precision: 18, scale: 4, nullable: false),
                    CLOSE_PRICE = table.Column<decimal>(type: "DECIMAL(18,4)", precision: 18, scale: 4, nullable: false),
                    HIGH_PRICE = table.Column<decimal>(type: "DECIMAL(18,4)", precision: 18, scale: 4, nullable: false),
                    LOW_PRICE = table.Column<decimal>(type: "DECIMAL(18,4)", precision: 18, scale: 4, nullable: false),
                    VOLUME = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    CHANGE_PERCENT = table.Column<decimal>(type: "DECIMAL(8,4)", precision: 8, scale: 4, nullable: false),
                    REGION = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    MARKET_TYPE = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    CREATED_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UPDATED_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MARKET_DATA", x => x.DATA_ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BUDGET_RECORDS_BUDGET_YEAR_BUDGET_MONTH",
                table: "BUDGET_RECORDS",
                columns: new[] { "BUDGET_YEAR", "BUDGET_MONTH" });

            migrationBuilder.CreateIndex(
                name: "IX_BUDGET_RECORDS_DEPARTMENT",
                table: "BUDGET_RECORDS",
                column: "DEPARTMENT");

            migrationBuilder.CreateIndex(
                name: "IX_BUDGET_RECORDS_REGION",
                table: "BUDGET_RECORDS",
                column: "REGION");

            migrationBuilder.CreateIndex(
                name: "IX_COMPANY_ANNOUNCEMENTS_ANNOUNCEMENT_DATE",
                table: "COMPANY_ANNOUNCEMENTS",
                column: "ANNOUNCEMENT_DATE");

            migrationBuilder.CreateIndex(
                name: "IX_COMPANY_ANNOUNCEMENTS_ANNOUNCEMENT_TYPE",
                table: "COMPANY_ANNOUNCEMENTS",
                column: "ANNOUNCEMENT_TYPE");

            migrationBuilder.CreateIndex(
                name: "IX_COMPANY_ANNOUNCEMENTS_REGION",
                table: "COMPANY_ANNOUNCEMENTS",
                column: "REGION");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEES_DEPARTMENT",
                table: "EMPLOYEES",
                column: "DEPARTMENT");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEES_REGION",
                table: "EMPLOYEES",
                column: "REGION");

            migrationBuilder.CreateIndex(
                name: "IX_FINANCIAL_RECORDS_EMPLOYEE_ID",
                table: "FINANCIAL_RECORDS",
                column: "EMPLOYEE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_FINANCIAL_RECORDS_EMPLOYEE_ID_RECORD_DATE",
                table: "FINANCIAL_RECORDS",
                columns: new[] { "EMPLOYEE_ID", "RECORD_DATE" });

            migrationBuilder.CreateIndex(
                name: "IX_FINANCIAL_RECORDS_RECORD_DATE",
                table: "FINANCIAL_RECORDS",
                column: "RECORD_DATE");

            migrationBuilder.CreateIndex(
                name: "IX_FINANCIAL_RECORDS_REGION",
                table: "FINANCIAL_RECORDS",
                column: "REGION");

            migrationBuilder.CreateIndex(
                name: "IX_MARKET_DATA_MARKET_DATE",
                table: "MARKET_DATA",
                column: "MARKET_DATE");

            migrationBuilder.CreateIndex(
                name: "IX_MARKET_DATA_REGION",
                table: "MARKET_DATA",
                column: "REGION");

            migrationBuilder.CreateIndex(
                name: "IX_MARKET_DATA_SYMBOL",
                table: "MARKET_DATA",
                column: "SYMBOL");

            migrationBuilder.CreateIndex(
                name: "IX_MARKET_DATA_SYMBOL_MARKET_DATE",
                table: "MARKET_DATA",
                columns: new[] { "SYMBOL", "MARKET_DATE" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BUDGET_RECORDS");

            migrationBuilder.DropTable(
                name: "COMPANY_ANNOUNCEMENTS");

            migrationBuilder.DropTable(
                name: "EMPLOYEES");

            migrationBuilder.DropTable(
                name: "FINANCIAL_RECORDS");

            migrationBuilder.DropTable(
                name: "MARKET_DATA");
        }
    }
}

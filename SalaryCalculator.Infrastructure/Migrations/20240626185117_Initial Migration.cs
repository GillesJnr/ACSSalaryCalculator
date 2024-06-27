using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalaryCalculator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IncomeTaxRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Year = table.Column<string>(type: "TEXT", nullable: true),
                    ChargeableIncome = table.Column<double>(type: "REAL", nullable: true),
                    Rate = table.Column<double>(type: "REAL", nullable: true),
                    TaxPayable = table.Column<double>(type: "REAL", nullable: true),
                    CumulativeIncome = table.Column<double>(type: "REAL", nullable: true),
                    CumulativeTax = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomeTaxRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pensions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TierName = table.Column<string>(type: "TEXT", nullable: true),
                    EmployeeRate = table.Column<double>(type: "REAL", nullable: true),
                    EmployerRate = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pensions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Salaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GrossSalary = table.Column<double>(type: "REAL", nullable: false),
                    BasicSalary = table.Column<double>(type: "REAL", nullable: false),
                    TotalPAYETax = table.Column<double>(type: "REAL", nullable: false),
                    EmployeePensionContribution = table.Column<double>(type: "REAL", nullable: false),
                    EmployerPensionContribution = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: true),
                    Password = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncomeTaxRates");

            migrationBuilder.DropTable(
                name: "Pensions");

            migrationBuilder.DropTable(
                name: "Salaries");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

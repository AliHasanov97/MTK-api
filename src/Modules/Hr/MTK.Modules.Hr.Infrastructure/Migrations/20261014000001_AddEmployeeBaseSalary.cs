using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Hr.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeBaseSalary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BaseSalary",
                schema: "hr",
                table: "employees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseSalary",
                schema: "hr",
                table: "employees");
        }
    }
}

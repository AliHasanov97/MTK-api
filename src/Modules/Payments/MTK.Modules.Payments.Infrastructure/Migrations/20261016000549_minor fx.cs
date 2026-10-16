using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Payments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class minorfx : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MaxDeductionAmount",
                schema: "payments",
                table: "payroll_deductions",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxDeductionAmount",
                schema: "payments",
                table: "payroll_deductions");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTK.Modules.Hr.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVacationReturnOrderUnusedDaysReturned : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UnusedDaysReturned",
                schema: "hr",
                table: "vacation_return_orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnusedDaysReturned",
                schema: "hr",
                table: "vacation_return_orders");
        }
    }
}

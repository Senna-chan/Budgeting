using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Budgeting.Migrations
{
    /// <inheritdoc />
    public partial class BudgetAddons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "BudgetEntry",
                type: "longtext",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "BudgetEntry");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Budgeting.Migrations
{
    /// <inheritdoc />
    public partial class variableCosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FixedEntry",
                table: "BudgetEntry",
                newName: "VariableCosts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VariableCosts",
                table: "BudgetEntry",
                newName: "FixedEntry");
        }
    }
}

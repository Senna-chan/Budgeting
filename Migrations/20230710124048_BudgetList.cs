using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Budgeting.Migrations
{
    /// <inheritdoc />
    public partial class BudgetList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BudgetListId",
                table: "BudgetEntry",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BudgetList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetList", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetEntry_BudgetListId",
                table: "BudgetEntry",
                column: "BudgetListId");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetEntry_BudgetList_BudgetListId",
                table: "BudgetEntry",
                column: "BudgetListId",
                principalTable: "BudgetList",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetEntry_BudgetList_BudgetListId",
                table: "BudgetEntry");

            migrationBuilder.DropTable(
                name: "BudgetList");

            migrationBuilder.DropIndex(
                name: "IX_BudgetEntry_BudgetListId",
                table: "BudgetEntry");

            migrationBuilder.DropColumn(
                name: "BudgetListId",
                table: "BudgetEntry");
        }
    }
}

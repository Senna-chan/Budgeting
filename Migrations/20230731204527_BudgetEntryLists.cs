using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Budgeting.Migrations
{
    /// <inheritdoc />
    public partial class BudgetEntryLists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetEntry_BudgetList_BudgetListId",
                table: "BudgetEntry");

            migrationBuilder.DropIndex(
                name: "IX_BudgetEntry_BudgetListId",
                table: "BudgetEntry");

            migrationBuilder.DropColumn(
                name: "BudgetListId",
                table: "BudgetEntry");

            migrationBuilder.CreateTable(
                name: "BudgetEntryLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    BudgetEntryId = table.Column<int>(type: "int", nullable: false),
                    BudgetListId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetEntryLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetEntryLists_BudgetEntry_BudgetEntryId",
                        column: x => x.BudgetEntryId,
                        principalTable: "BudgetEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetEntryLists_BudgetList_BudgetListId",
                        column: x => x.BudgetListId,
                        principalTable: "BudgetList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetEntryLists_BudgetEntryId",
                table: "BudgetEntryLists",
                column: "BudgetEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetEntryLists_BudgetListId",
                table: "BudgetEntryLists",
                column: "BudgetListId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetEntryLists");

            migrationBuilder.AddColumn<int>(
                name: "BudgetListId",
                table: "BudgetEntry",
                type: "int",
                nullable: true);

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
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Budgeting.Migrations
{
    /// <inheritdoc />
    public partial class MigrationBugFound : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BudgetEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    Category = table.Column<string>(type: "longtext", nullable: true),
                    IsIncome = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MoneyAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TimeAmount = table.Column<int>(type: "int", nullable: false),
                    FromCreditcard = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ToSharedAccount = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    VariableCosts = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TransferTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Description = table.Column<string>(type: "longtext", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetEntry", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BudgetList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetList", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BudgetEntryBudgetList",
                columns: table => new
                {
                    BudgetEntriesId = table.Column<int>(type: "int", nullable: false),
                    ListsPartOfId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetEntryBudgetList", x => new { x.BudgetEntriesId, x.ListsPartOfId });
                    table.ForeignKey(
                        name: "FK_BudgetEntryBudgetList_BudgetEntry_BudgetEntriesId",
                        column: x => x.BudgetEntriesId,
                        principalTable: "BudgetEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetEntryBudgetList_BudgetList_ListsPartOfId",
                        column: x => x.ListsPartOfId,
                        principalTable: "BudgetList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetEntryBudgetList_ListsPartOfId",
                table: "BudgetEntryBudgetList",
                column: "ListsPartOfId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetEntryBudgetList");

            migrationBuilder.DropTable(
                name: "BudgetEntry");

            migrationBuilder.DropTable(
                name: "BudgetList");
        }
    }
}

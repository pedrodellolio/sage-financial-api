using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SageFinancialAPI.Migrations
{
    /// <inheritdoc />
    public partial class TransactionLabelUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransactionLabels");

            migrationBuilder.AddColumn<Guid>(
                name: "LabelId",
                table: "Transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_LabelId",
                table: "Transactions",
                column: "LabelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Labels_LabelId",
                table: "Transactions",
                column: "LabelId",
                principalTable: "Labels",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Labels_LabelId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_LabelId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "LabelId",
                table: "Transactions");

            migrationBuilder.CreateTable(
                name: "TransactionLabels",
                columns: table => new
                {
                    LabelsId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionLabels", x => new { x.LabelsId, x.TransactionsId });
                    table.ForeignKey(
                        name: "FK_TransactionLabels_Labels_LabelsId",
                        column: x => x.LabelsId,
                        principalTable: "Labels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransactionLabels_Transactions_TransactionsId",
                        column: x => x.TransactionsId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLabels_TransactionsId",
                table: "TransactionLabels",
                column: "TransactionsId");
        }
    }
}

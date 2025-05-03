using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SageFinancialAPI.Migrations
{
    /// <inheritdoc />
    public partial class NotificationBudgetGOal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Transactions_TransactionId",
                table: "Notifications");

            migrationBuilder.AlterColumn<Guid>(
                name: "TransactionId",
                table: "Notifications",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "BudgetGoalId",
                table: "Notifications",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_BudgetGoalId",
                table: "Notifications",
                column: "BudgetGoalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_BudgetGoals_BudgetGoalId",
                table: "Notifications",
                column: "BudgetGoalId",
                principalTable: "BudgetGoals",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Transactions_TransactionId",
                table: "Notifications",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_BudgetGoals_BudgetGoalId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Transactions_TransactionId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_BudgetGoalId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "BudgetGoalId",
                table: "Notifications");

            migrationBuilder.AlterColumn<Guid>(
                name: "TransactionId",
                table: "Notifications",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Transactions_TransactionId",
                table: "Notifications",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

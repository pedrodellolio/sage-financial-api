using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SageFinancialAPI.Migrations
{
    /// <inheritdoc />
    public partial class UniqueProfileTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budget_Profile_ProfileId",
                table: "Budget");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetGoal_Budget_BudgetId",
                table: "BudgetGoal");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetGoal_Labels_LabelId",
                table: "BudgetGoal");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_Profile_ProfileId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Labels_Profile_ProfileId",
                table: "Labels");

            migrationBuilder.DropForeignKey(
                name: "FK_LabelTransaction_Labels_LabelsId",
                table: "LabelTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_LabelTransaction_Transactions_TransactionsId",
                table: "LabelTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Profile_Users_UserId",
                table: "Profile");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Wallet_WalletId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Wallet_Profile_ProfileId",
                table: "Wallet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Wallet",
                table: "Wallet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Profile",
                table: "Profile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LabelTransaction",
                table: "LabelTransaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BudgetGoal",
                table: "BudgetGoal");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Budget",
                table: "Budget");

            migrationBuilder.RenameTable(
                name: "Wallet",
                newName: "Wallets");

            migrationBuilder.RenameTable(
                name: "Profile",
                newName: "Profiles");

            migrationBuilder.RenameTable(
                name: "LabelTransaction",
                newName: "TransactionLabels");

            migrationBuilder.RenameTable(
                name: "BudgetGoal",
                newName: "BudgetGoals");

            migrationBuilder.RenameTable(
                name: "Budget",
                newName: "Budgets");

            migrationBuilder.RenameIndex(
                name: "IX_Wallet_ProfileId",
                table: "Wallets",
                newName: "IX_Wallets_ProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Profile_UserId",
                table: "Profiles",
                newName: "IX_Profiles_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_LabelTransaction_TransactionsId",
                table: "TransactionLabels",
                newName: "IX_TransactionLabels_TransactionsId");

            migrationBuilder.RenameIndex(
                name: "IX_BudgetGoal_LabelId",
                table: "BudgetGoals",
                newName: "IX_BudgetGoals_LabelId");

            migrationBuilder.RenameIndex(
                name: "IX_BudgetGoal_BudgetId",
                table: "BudgetGoals",
                newName: "IX_BudgetGoals_BudgetId");

            migrationBuilder.RenameIndex(
                name: "IX_Budget_ProfileId",
                table: "Budgets",
                newName: "IX_Budgets_ProfileId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Wallets",
                table: "Wallets",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Profiles",
                table: "Profiles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionLabels",
                table: "TransactionLabels",
                columns: new[] { "LabelsId", "TransactionsId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_BudgetGoals",
                table: "BudgetGoals",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Budgets",
                table: "Budgets",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_Title",
                table: "Profiles",
                column: "Title",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetGoals_Budgets_BudgetId",
                table: "BudgetGoals",
                column: "BudgetId",
                principalTable: "Budgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetGoals_Labels_LabelId",
                table: "BudgetGoals",
                column: "LabelId",
                principalTable: "Labels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Budgets_Profiles_ProfileId",
                table: "Budgets",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Profiles_ProfileId",
                table: "Files",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Labels_Profiles_ProfileId",
                table: "Labels",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_Users_UserId",
                table: "Profiles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionLabels_Labels_LabelsId",
                table: "TransactionLabels",
                column: "LabelsId",
                principalTable: "Labels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionLabels_Transactions_TransactionsId",
                table: "TransactionLabels",
                column: "TransactionsId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Wallets_WalletId",
                table: "Transactions",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_Profiles_ProfileId",
                table: "Wallets",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetGoals_Budgets_BudgetId",
                table: "BudgetGoals");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetGoals_Labels_LabelId",
                table: "BudgetGoals");

            migrationBuilder.DropForeignKey(
                name: "FK_Budgets_Profiles_ProfileId",
                table: "Budgets");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_Profiles_ProfileId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Labels_Profiles_ProfileId",
                table: "Labels");

            migrationBuilder.DropForeignKey(
                name: "FK_Profiles_Users_UserId",
                table: "Profiles");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionLabels_Labels_LabelsId",
                table: "TransactionLabels");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionLabels_Transactions_TransactionsId",
                table: "TransactionLabels");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Wallets_WalletId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_Profiles_ProfileId",
                table: "Wallets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Wallets",
                table: "Wallets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionLabels",
                table: "TransactionLabels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Profiles",
                table: "Profiles");

            migrationBuilder.DropIndex(
                name: "IX_Profiles_Title",
                table: "Profiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Budgets",
                table: "Budgets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BudgetGoals",
                table: "BudgetGoals");

            migrationBuilder.RenameTable(
                name: "Wallets",
                newName: "Wallet");

            migrationBuilder.RenameTable(
                name: "TransactionLabels",
                newName: "LabelTransaction");

            migrationBuilder.RenameTable(
                name: "Profiles",
                newName: "Profile");

            migrationBuilder.RenameTable(
                name: "Budgets",
                newName: "Budget");

            migrationBuilder.RenameTable(
                name: "BudgetGoals",
                newName: "BudgetGoal");

            migrationBuilder.RenameIndex(
                name: "IX_Wallets_ProfileId",
                table: "Wallet",
                newName: "IX_Wallet_ProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionLabels_TransactionsId",
                table: "LabelTransaction",
                newName: "IX_LabelTransaction_TransactionsId");

            migrationBuilder.RenameIndex(
                name: "IX_Profiles_UserId",
                table: "Profile",
                newName: "IX_Profile_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Budgets_ProfileId",
                table: "Budget",
                newName: "IX_Budget_ProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_BudgetGoals_LabelId",
                table: "BudgetGoal",
                newName: "IX_BudgetGoal_LabelId");

            migrationBuilder.RenameIndex(
                name: "IX_BudgetGoals_BudgetId",
                table: "BudgetGoal",
                newName: "IX_BudgetGoal_BudgetId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Wallet",
                table: "Wallet",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LabelTransaction",
                table: "LabelTransaction",
                columns: new[] { "LabelsId", "TransactionsId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Profile",
                table: "Profile",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Budget",
                table: "Budget",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BudgetGoal",
                table: "BudgetGoal",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Budget_Profile_ProfileId",
                table: "Budget",
                column: "ProfileId",
                principalTable: "Profile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetGoal_Budget_BudgetId",
                table: "BudgetGoal",
                column: "BudgetId",
                principalTable: "Budget",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetGoal_Labels_LabelId",
                table: "BudgetGoal",
                column: "LabelId",
                principalTable: "Labels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Profile_ProfileId",
                table: "Files",
                column: "ProfileId",
                principalTable: "Profile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Labels_Profile_ProfileId",
                table: "Labels",
                column: "ProfileId",
                principalTable: "Profile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LabelTransaction_Labels_LabelsId",
                table: "LabelTransaction",
                column: "LabelsId",
                principalTable: "Labels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LabelTransaction_Transactions_TransactionsId",
                table: "LabelTransaction",
                column: "TransactionsId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Profile_Users_UserId",
                table: "Profile",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Wallet_WalletId",
                table: "Transactions",
                column: "WalletId",
                principalTable: "Wallet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Wallet_Profile_ProfileId",
                table: "Wallet",
                column: "ProfileId",
                principalTable: "Profile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

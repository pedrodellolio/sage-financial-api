using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SageFinancialAPI.Migrations
{
    /// <inheritdoc />
    public partial class TransactionInterestUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "InterestPercentage",
                table: "Transactions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InterestPercentage",
                table: "Transactions");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SageFinancialAPI.Migrations
{
    /// <inheritdoc />
    public partial class UniqueMonthYearProfileWallet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Wallets_Month_Year",
                table: "Wallets");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_Month_Year_ProfileId",
                table: "Wallets",
                columns: new[] { "Month", "Year", "ProfileId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Wallets_Month_Year_ProfileId",
                table: "Wallets");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_Month_Year",
                table: "Wallets",
                columns: new[] { "Month", "Year" },
                unique: true);
        }
    }
}

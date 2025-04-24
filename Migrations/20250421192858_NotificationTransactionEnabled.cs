using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SageFinancialAPI.Migrations
{
    /// <inheritdoc />
    public partial class NotificationTransactionEnabled : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "Notifications",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "Notifications");
        }
    }
}

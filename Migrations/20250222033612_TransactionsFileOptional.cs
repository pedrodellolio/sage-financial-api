using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SageFinancialAPI.Migrations
{
    /// <inheritdoc />
    public partial class TransactionsFileOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Files_FileId",
                table: "Transactions");

            migrationBuilder.AlterColumn<Guid>(
                name: "FileId",
                table: "Transactions",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Files_FileId",
                table: "Transactions",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Files_FileId",
                table: "Transactions");

            migrationBuilder.AlterColumn<Guid>(
                name: "FileId",
                table: "Transactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Files_FileId",
                table: "Transactions",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

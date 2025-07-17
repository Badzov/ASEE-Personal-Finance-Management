using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pfm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CatCodeFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CategoryCode",
                table: "Transactions",
                type: "nvarchar(4)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CategoryCode",
                table: "Transactions",
                column: "CategoryCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Categories_CategoryCode",
                table: "Transactions",
                column: "CategoryCode",
                principalTable: "Categories",
                principalColumn: "Code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Categories_CategoryCode",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_CategoryCode",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CategoryCode",
                table: "Transactions");
        }
    }
}

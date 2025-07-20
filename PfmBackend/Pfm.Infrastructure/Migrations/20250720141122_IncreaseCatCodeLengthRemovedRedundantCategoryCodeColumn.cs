using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pfm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IncreaseCatCodeLengthRemovedRedundantCategoryCodeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
           name: "FK_Categories_Categories_ParentCode",
           table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Splits_Categories_CatCode",
                table: "Splits");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Categories_CategoryCode",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Categories_CatCode",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_CategoryCode",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CategoryCode",
                table: "Transactions");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Categories",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4)",
                oldMaxLength: 4);

            migrationBuilder.AlterColumn<string>(
                name: "ParentCode",
                table: "Categories",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true, 
                oldClrType: typeof(string),
                oldType: "nvarchar(4)",
                oldMaxLength: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CatCode",
                table: "Splits",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4)",
                oldMaxLength: 4);

            migrationBuilder.AlterColumn<string>(
                name: "CatCode",
                table: "Transactions",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(4)",
                oldMaxLength: 4,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Categories_ParentCode",
                table: "Categories",
                column: "ParentCode",
                principalTable: "Categories",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Splits_Categories_CatCode",
                table: "Splits",
                column: "CatCode",
                principalTable: "Categories",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Categories_CatCode",
                table: "Transactions",
                column: "CatCode",
                principalTable: "Categories",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropForeignKey(
            name: "FK_Categories_Categories_ParentCode",
            table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Splits_Categories_CatCode",
                table: "Splits");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Categories_CatCode",
                table: "Transactions");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Categories",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(5)",
                oldMaxLength: 5);

            migrationBuilder.AlterColumn<string>(
                name: "ParentCode",
                table: "Categories",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(5)",
                oldMaxLength: 5,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CatCode",
                table: "Splits",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(5)",
                oldMaxLength: 5);

            migrationBuilder.AlterColumn<string>(
                name: "CatCode",
                table: "Transactions",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(5)",
                oldMaxLength: 5,
                oldNullable: true);


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
                name: "FK_Categories_Categories_ParentCode",
                table: "Categories",
                column: "ParentCode",
                principalTable: "Categories",
                principalColumn: "Code");

            migrationBuilder.AddForeignKey(
                name: "FK_Splits_Categories_CatCode",
                table: "Splits",
                column: "CatCode",
                principalTable: "Categories",
                principalColumn: "Code");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Categories_CatCode",
                table: "Transactions",
                column: "CatCode",
                principalTable: "Categories",
                principalColumn: "Code");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Categories_CategoryCode",
                table: "Transactions",
                column: "CategoryCode",
                principalTable: "Categories",
                principalColumn: "Code");
        }
    }
}



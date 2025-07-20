using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pfm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSplitCategoryMarker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Code", "Name", "ParentCode" },
                values: new object[] { "SPLIT", "Split Transaction", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Code",
                keyValue: "SPLIT");
        }
    }
}

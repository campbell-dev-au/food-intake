using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodIntake.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCategorySiblingNameIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Categories_ParentId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_SchemeId",
                table: "Categories");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentId_Name",
                table: "Categories",
                columns: new[] { "ParentId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_SchemeId_Name",
                table: "Categories",
                columns: new[] { "SchemeId", "Name" },
                unique: true,
                filter: "\"ParentId\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Categories_ParentId_Name",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_SchemeId_Name",
                table: "Categories");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentId",
                table: "Categories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_SchemeId",
                table: "Categories",
                column: "SchemeId");
        }
    }
}

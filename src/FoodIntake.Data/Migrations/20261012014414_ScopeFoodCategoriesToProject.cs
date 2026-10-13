using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodIntake.Data.Migrations
{
    /// <inheritdoc />
    public partial class ScopeFoodCategoriesToProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FoodCategories",
                table: "FoodCategories");

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "FoodCategories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FoodCategories",
                table: "FoodCategories",
                columns: new[] { "ProjectId", "FoodId", "CategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_FoodCategories_FoodId",
                table: "FoodCategories",
                column: "FoodId");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodCategories_Projects_ProjectId",
                table: "FoodCategories",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodCategories_Projects_ProjectId",
                table: "FoodCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FoodCategories",
                table: "FoodCategories");

            migrationBuilder.DropIndex(
                name: "IX_FoodCategories_FoodId",
                table: "FoodCategories");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "FoodCategories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FoodCategories",
                table: "FoodCategories",
                columns: new[] { "FoodId", "CategoryId" });
        }
    }
}

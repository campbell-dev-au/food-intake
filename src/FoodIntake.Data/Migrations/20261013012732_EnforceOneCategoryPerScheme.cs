using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodIntake.Data.Migrations
{
    /// <inheritdoc />
    public partial class EnforceOneCategoryPerScheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodCategories_Categories_CategoryId",
                table: "FoodCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FoodCategories",
                table: "FoodCategories");

            migrationBuilder.DropIndex(
                name: "IX_FoodCategories_CategoryId",
                table: "FoodCategories");

            migrationBuilder.Sql(
                "CREATE UNIQUE INDEX \"TMP_Categories_Id_SchemeId\" ON \"Categories\" (\"Id\", \"SchemeId\");");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Categories_Id_SchemeId",
                table: "Categories",
                columns: new[] { "Id", "SchemeId" });

            migrationBuilder.AddColumn<int>(
                name: "SchemeId",
                table: "FoodCategories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            // 1. Fill SchemeId from each row's category.
            migrationBuilder.Sql(@"
    UPDATE FoodCategories
    SET SchemeId = (SELECT c.SchemeId FROM Categories c WHERE c.Id = FoodCategories.CategoryId);");

            // 2. Where a food has several categories in one scheme, keep the lowest CategoryId.
            migrationBuilder.Sql(@"
    DELETE FROM FoodCategories
    WHERE EXISTS (
        SELECT 1 FROM FoodCategories other
        WHERE other.ProjectId = FoodCategories.ProjectId
          AND other.FoodId = FoodCategories.FoodId
          AND other.SchemeId = FoodCategories.SchemeId
          AND other.CategoryId < FoodCategories.CategoryId);");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FoodCategories",
                table: "FoodCategories",
                columns: new[] { "ProjectId", "FoodId", "SchemeId" });

            migrationBuilder.CreateIndex(
                name: "IX_FoodCategories_CategoryId_SchemeId",
                table: "FoodCategories",
                columns: new[] { "CategoryId", "SchemeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_FoodCategories_Categories_CategoryId_SchemeId",
                table: "FoodCategories",
                columns: new[] { "CategoryId", "SchemeId" },
                principalTable: "Categories",
                principalColumns: new[] { "Id", "SchemeId" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodCategories_Categories_CategoryId_SchemeId",
                table: "FoodCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FoodCategories",
                table: "FoodCategories");

            migrationBuilder.DropIndex(
                name: "IX_FoodCategories_CategoryId_SchemeId",
                table: "FoodCategories");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Categories_Id_SchemeId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "SchemeId",
                table: "FoodCategories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FoodCategories",
                table: "FoodCategories",
                columns: new[] { "ProjectId", "FoodId", "CategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_FoodCategories_CategoryId",
                table: "FoodCategories",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodCategories_Categories_CategoryId",
                table: "FoodCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

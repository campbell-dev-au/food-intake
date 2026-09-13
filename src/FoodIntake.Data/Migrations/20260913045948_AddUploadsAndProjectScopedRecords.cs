using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodIntake.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUploadsAndProjectScopedRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodCategories_Food_FoodId",
                table: "FoodCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_Lines_Food_FoodId",
                table: "Lines");

            migrationBuilder.DropIndex(
                name: "IX_FoodRecords_RemoteResourceId",
                table: "FoodRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Food",
                table: "Food");

            migrationBuilder.RenameTable(
                name: "Food",
                newName: "Foods");

            migrationBuilder.RenameIndex(
                name: "IX_Food_DataSourceId_ExternalFoodId",
                table: "Foods",
                newName: "IX_Foods_DataSourceId_ExternalFoodId");

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "FoodRecords",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Foods",
                table: "Foods",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Uploads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TimePointId = table.Column<int>(type: "INTEGER", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    ImportedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LineCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uploads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Uploads_TimePoints_TimePointId",
                        column: x => x.TimePointId,
                        principalTable: "TimePoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoodRecords_ProjectId_RemoteResourceId",
                table: "FoodRecords",
                columns: new[] { "ProjectId", "RemoteResourceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Uploads_TimePointId",
                table: "Uploads",
                column: "TimePointId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FoodCategories_Foods_FoodId",
                table: "FoodCategories",
                column: "FoodId",
                principalTable: "Foods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lines_Foods_FoodId",
                table: "Lines",
                column: "FoodId",
                principalTable: "Foods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodCategories_Foods_FoodId",
                table: "FoodCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_Lines_Foods_FoodId",
                table: "Lines");

            migrationBuilder.DropTable(
                name: "Uploads");

            migrationBuilder.DropIndex(
                name: "IX_FoodRecords_ProjectId_RemoteResourceId",
                table: "FoodRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Foods",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "FoodRecords");

            migrationBuilder.RenameTable(
                name: "Foods",
                newName: "Food");

            migrationBuilder.RenameIndex(
                name: "IX_Foods_DataSourceId_ExternalFoodId",
                table: "Food",
                newName: "IX_Food_DataSourceId_ExternalFoodId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Food",
                table: "Food",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_FoodRecords_RemoteResourceId",
                table: "FoodRecords",
                column: "RemoteResourceId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FoodCategories_Food_FoodId",
                table: "FoodCategories",
                column: "FoodId",
                principalTable: "Food",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lines_Food_FoodId",
                table: "Lines",
                column: "FoodId",
                principalTable: "Food",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodIntake.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RemoteClientId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Foods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DataSourceId = table.Column<string>(type: "TEXT", nullable: false),
                    ExternalFoodId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Foods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MealSections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealSections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Nutrients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: false),
                    Unit = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nutrients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Schemes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schemes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectClients",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectClients", x => new { x.ProjectId, x.ClientId });
                    table.ForeignKey(
                        name: "FK_ProjectClients_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectClients_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimePoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimePoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimePoints_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SchemeId = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentId = table.Column<int>(type: "INTEGER", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Categories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Categories_Schemes_SchemeId",
                        column: x => x.SchemeId,
                        principalTable: "Schemes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FoodRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false),
                    TimePointId = table.Column<int>(type: "INTEGER", nullable: false),
                    RemoteResourceId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodRecords_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FoodRecords_TimePoints_TimePointId",
                        column: x => x.TimePointId,
                        principalTable: "TimePoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "FoodCategories",
                columns: table => new
                {
                    FoodId = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    AssignedBy = table.Column<string>(type: "TEXT", nullable: false),
                    Confidence = table.Column<decimal>(type: "TEXT", nullable: true),
                    ConfirmedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodCategories", x => new { x.FoodId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_FoodCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FoodCategories_Foods_FoodId",
                        column: x => x.FoodId,
                        principalTable: "Foods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FoodId = table.Column<int>(type: "INTEGER", nullable: false),
                    FoodRecordId = table.Column<int>(type: "INTEGER", nullable: false),
                    DayName = table.Column<string>(type: "TEXT", nullable: false),
                    DayDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    MealSectionId = table.Column<int>(type: "INTEGER", nullable: false),
                    WeightG = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lines_FoodRecords_FoodRecordId",
                        column: x => x.FoodRecordId,
                        principalTable: "FoodRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Lines_Foods_FoodId",
                        column: x => x.FoodId,
                        principalTable: "Foods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Lines_MealSections_MealSectionId",
                        column: x => x.MealSectionId,
                        principalTable: "MealSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LineNutrients",
                columns: table => new
                {
                    ConsumptionLineId = table.Column<int>(type: "INTEGER", nullable: false),
                    NutrientId = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineNutrients", x => new { x.ConsumptionLineId, x.NutrientId });
                    table.ForeignKey(
                        name: "FK_LineNutrients_Lines_ConsumptionLineId",
                        column: x => x.ConsumptionLineId,
                        principalTable: "Lines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LineNutrients_Nutrients_NutrientId",
                        column: x => x.NutrientId,
                        principalTable: "Nutrients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentId",
                table: "Categories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_SchemeId",
                table: "Categories",
                column: "SchemeId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_RemoteClientId",
                table: "Clients",
                column: "RemoteClientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FoodCategories_CategoryId",
                table: "FoodCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodRecords_ClientId",
                table: "FoodRecords",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodRecords_ProjectId_RemoteResourceId",
                table: "FoodRecords",
                columns: new[] { "ProjectId", "RemoteResourceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FoodRecords_TimePointId",
                table: "FoodRecords",
                column: "TimePointId");

            migrationBuilder.CreateIndex(
                name: "IX_Foods_DataSourceId_ExternalFoodId",
                table: "Foods",
                columns: new[] { "DataSourceId", "ExternalFoodId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LineNutrients_NutrientId",
                table: "LineNutrients",
                column: "NutrientId");

            migrationBuilder.CreateIndex(
                name: "IX_Lines_FoodId",
                table: "Lines",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_Lines_FoodRecordId",
                table: "Lines",
                column: "FoodRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_Lines_MealSectionId",
                table: "Lines",
                column: "MealSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_MealSections_Name",
                table: "MealSections",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Nutrients_Code",
                table: "Nutrients",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectClients_ClientId",
                table: "ProjectClients",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Name",
                table: "Projects",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schemes_Name",
                table: "Schemes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimePoints_ProjectId_Name",
                table: "TimePoints",
                columns: new[] { "ProjectId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Uploads_TimePointId",
                table: "Uploads",
                column: "TimePointId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodCategories");

            migrationBuilder.DropTable(
                name: "LineNutrients");

            migrationBuilder.DropTable(
                name: "ProjectClients");

            migrationBuilder.DropTable(
                name: "Uploads");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Lines");

            migrationBuilder.DropTable(
                name: "Nutrients");

            migrationBuilder.DropTable(
                name: "Schemes");

            migrationBuilder.DropTable(
                name: "FoodRecords");

            migrationBuilder.DropTable(
                name: "Foods");

            migrationBuilder.DropTable(
                name: "MealSections");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "TimePoints");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}

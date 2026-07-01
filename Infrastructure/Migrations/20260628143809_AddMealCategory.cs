using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMealCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MealCategoryId",
                table: "Meals",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MealCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Meals_MealCategoryId",
                table: "Meals",
                column: "MealCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Meals_MealCategories_MealCategoryId",
                table: "Meals",
                column: "MealCategoryId",
                principalTable: "MealCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meals_MealCategories_MealCategoryId",
                table: "Meals");

            migrationBuilder.DropTable(
                name: "MealCategories");

            migrationBuilder.DropIndex(
                name: "IX_Meals_MealCategoryId",
                table: "Meals");

            migrationBuilder.DropColumn(
                name: "MealCategoryId",
                table: "Meals");
        }
    }
}

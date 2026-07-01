using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRefToMealCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meals_MealCategories_MealCategoryId",
                table: "Meals");

            migrationBuilder.AlterColumn<int>(
                name: "MealCategoryId",
                table: "Meals",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Meals_MealCategories_MealCategoryId",
                table: "Meals",
                column: "MealCategoryId",
                principalTable: "MealCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meals_MealCategories_MealCategoryId",
                table: "Meals");

            migrationBuilder.AlterColumn<int>(
                name: "MealCategoryId",
                table: "Meals",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Meals_MealCategories_MealCategoryId",
                table: "Meals",
                column: "MealCategoryId",
                principalTable: "MealCategories",
                principalColumn: "Id");
        }
    }
}

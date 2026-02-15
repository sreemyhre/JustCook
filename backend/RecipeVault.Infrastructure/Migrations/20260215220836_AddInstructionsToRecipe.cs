using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeVault.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInstructionsToRecipe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Instructions",
                table: "Recipes",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Instructions",
                table: "Recipes");
        }
    }
}

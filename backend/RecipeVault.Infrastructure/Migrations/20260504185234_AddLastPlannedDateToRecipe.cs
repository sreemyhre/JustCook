using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeVault.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLastPlannedDateToRecipe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Recipes_Rotation",
                table: "Recipes");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastPlannedDate",
                table: "Recipes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_Rotation",
                table: "Recipes",
                columns: new[] { "UserId", "LastCookedDate", "LastPlannedDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Recipes_Rotation",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "LastPlannedDate",
                table: "Recipes");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_Rotation",
                table: "Recipes",
                columns: new[] { "UserId", "LastCookedDate" });
        }
    }
}

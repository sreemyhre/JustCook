using Microsoft.EntityFrameworkCore;
using RecipeVault.Core.Entities;
using RecipeVault.Infrastructure.Data;
using RecipeVault.Infrastructure.Repositories;
using Xunit;

namespace RecipeVault.Tests;

public class RecipeRepositoryTests
{
    private AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ShouldAddRecipeToDatabase()
    {
        using var context = CreateInMemoryContext();
        var repo = new RecipeRepository(context);
        var recipe = new Recipe { Name = "Pasta", UserId = 1, ServingSize = 4 };

        await repo.AddAsync(recipe);

        var saved = await context.Recipes.FirstOrDefaultAsync();
        Assert.NotNull(saved);
        Assert.Equal("Pasta", saved.Name);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnTheAddedRecipe()
    {
        using var context = CreateInMemoryContext();
        var repo = new RecipeRepository(context);
        var recipe = new Recipe { Name = "Tacos", UserId = 1, ServingSize = 2 };

        var result = await repo.AddAsync(recipe);

        Assert.Equal("Tacos", result.Name);
        Assert.Equal(1, result.UserId);
    }

    [Fact]
    public async Task GetByIdAsync_WhenRecipeExists_ShouldReturnRecipe()
    {
        using var context = CreateInMemoryContext();
        var recipe = new Recipe { Name = "Burger", UserId = 1, ServingSize = 1 };
        context.Recipes.Add(recipe);
        await context.SaveChangesAsync();

        var repo = new RecipeRepository(context);
        var result = await repo.GetByIdAsync(recipe.Id);

        Assert.NotNull(result);
        Assert.Equal("Burger", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenRecipeNotFound_ShouldReturnNull()
    {
        using var context = CreateInMemoryContext();
        var repo = new RecipeRepository(context);

        var result = await repo.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllByUserIdAsync_ShouldReturnOnlyUserRecipes()
    {
        using var context = CreateInMemoryContext();
        context.Recipes.Add(new Recipe { Name = "Pasta", UserId = 1 });
        context.Recipes.Add(new Recipe { Name = "Tacos", UserId = 1 });
        context.Recipes.Add(new Recipe { Name = "Burger", UserId = 2 });
        await context.SaveChangesAsync();

        var repo = new RecipeRepository(context);
        var result = (await repo.GetAllByUserIdAsync(1)).ToList();

        Assert.Equal(2, result.Count);
        Assert.All(result, r => Assert.Equal(1, r.UserId));
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistChanges()
    {
        using var context = CreateInMemoryContext();
        var recipe = new Recipe { Name = "Pasta", UserId = 1, ServingSize = 4 };
        context.Recipes.Add(recipe);
        await context.SaveChangesAsync();

        recipe.Name = "Updated Pasta";
        var repo = new RecipeRepository(context);
        await repo.UpdateAsync(recipe);

        var updated = await context.Recipes.FindAsync(recipe.Id);
        Assert.NotNull(updated);
        Assert.Equal("Updated Pasta", updated.Name);
    }

    [Fact]
    public async Task DeleteAsync_WhenRecipeExists_ShouldReturnTrue()
    {
        using var context = CreateInMemoryContext();
        var recipe = new Recipe { Name = "Pasta", UserId = 1 };
        context.Recipes.Add(recipe);
        await context.SaveChangesAsync();

        var repo = new RecipeRepository(context);
        var result = await repo.DeleteAsync(recipe.Id);

        Assert.True(result);
        Assert.Null(await context.Recipes.FindAsync(recipe.Id));
    }

    [Fact]
    public async Task DeleteAsync_WhenRecipeNotFound_ShouldReturnFalse()
    {
        using var context = CreateInMemoryContext();
        var repo = new RecipeRepository(context);

        var result = await repo.DeleteAsync(999);

        Assert.False(result);
    }

    [Fact]
    public async Task GetLeastRecentlyCookedAsync_ShouldReturnOrderedByLastCooked()
    {
        using var context = CreateInMemoryContext();
        context.Recipes.Add(new Recipe { Name = "Never Cooked", UserId = 1, LastCookedDate = null });
        context.Recipes.Add(new Recipe { Name = "Recent", UserId = 1, LastCookedDate = DateTime.UtcNow });
        context.Recipes.Add(new Recipe { Name = "Old", UserId = 1, LastCookedDate = DateTime.UtcNow.AddDays(-30) });
        await context.SaveChangesAsync();

        var repo = new RecipeRepository(context);
        var result = (await repo.GetLeastRecentlyCookedAsync(1, 3)).ToList();

        Assert.Equal(3, result.Count);
        Assert.Equal("Never Cooked", result[0].Name);
        Assert.Equal("Old", result[1].Name);
        Assert.Equal("Recent", result[2].Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldIncludeIngredients()
    {
        using var context = CreateInMemoryContext();
        var recipe = new Recipe
        {
            Name = "Pasta",
            UserId = 1,
            Ingredients = new List<Ingredient>
            {
                new() { Name = "Spaghetti", Quantity = 400, Unit = "g" },
                new() { Name = "Eggs", Quantity = 3 }
            }
        };
        context.Recipes.Add(recipe);
        await context.SaveChangesAsync();

        var repo = new RecipeRepository(context);
        var result = await repo.GetByIdAsync(recipe.Id);

        Assert.NotNull(result);
        Assert.Equal(2, result.Ingredients.Count);
    }

    [Fact]
    public async Task AddAsync_WithIngredients_ShouldSaveIngredients()
    {
        using var context = CreateInMemoryContext();
        var repo = new RecipeRepository(context);
        var recipe = new Recipe
        {
            Name = "Tacos",
            UserId = 1,
            Ingredients = new List<Ingredient>
            {
                new() { Name = "Tortillas", Quantity = 4, Unit = "pieces" },
                new() { Name = "Ground Beef", Quantity = 500, Unit = "g" }
            }
        };

        await repo.AddAsync(recipe);

        var saved = await context.Recipes.Include(r => r.Ingredients).FirstAsync();
        Assert.Equal(2, saved.Ingredients.Count);
    }
}

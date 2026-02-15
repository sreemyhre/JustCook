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
}

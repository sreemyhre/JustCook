using AutoMapper;
using Moq;
using RecipeVault.Application.DTOs;
using RecipeVault.Application.Services;
using RecipeVault.Core.Entities;
using RecipeVault.Core.Interfaces;
using Xunit;

namespace RecipeVault.Tests;

public class RecipeServiceTests
{
    private readonly Mock<IRecipeRepository> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly RecipeService _service;

    public RecipeServiceTests()
    {
        _mockRepo = new Mock<IRecipeRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new RecipeService(_mockRepo.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task CreateRecipeAsync_ShouldMapDtoToEntity()
    {
        var dto = new CreateRecipeDto { Name = "Pasta", UserId = 1 };
        var recipe = new Recipe { Name = "Pasta", UserId = 1 };
        var recipeDto = new RecipeDto { Id = 1, Name = "Pasta" };

        _mockMapper.Setup(m => m.Map<Recipe>(dto)).Returns(recipe);
        _mockRepo.Setup(r => r.AddAsync(recipe)).ReturnsAsync(recipe);
        _mockMapper.Setup(m => m.Map<RecipeDto>(recipe)).Returns(recipeDto);

        await _service.CreateRecipeAsync(dto);

        _mockMapper.Verify(m => m.Map<Recipe>(dto), Times.Once);
    }

    [Fact]
    public async Task CreateRecipeAsync_ShouldCallRepositoryAdd()
    {
        var dto = new CreateRecipeDto { Name = "Pasta", UserId = 1 };
        var recipe = new Recipe { Name = "Pasta", UserId = 1 };
        var recipeDto = new RecipeDto { Id = 1, Name = "Pasta" };

        _mockMapper.Setup(m => m.Map<Recipe>(dto)).Returns(recipe);
        _mockRepo.Setup(r => r.AddAsync(recipe)).ReturnsAsync(recipe);
        _mockMapper.Setup(m => m.Map<RecipeDto>(recipe)).Returns(recipeDto);

        await _service.CreateRecipeAsync(dto);

        _mockRepo.Verify(r => r.AddAsync(recipe), Times.Once);
    }

    [Fact]
    public async Task CreateRecipeAsync_ShouldReturnMappedRecipeDto()
    {
        var dto = new CreateRecipeDto { Name = "Pasta", UserId = 1 };
        var recipe = new Recipe { Name = "Pasta", UserId = 1 };
        var expected = new RecipeDto { Id = 1, Name = "Pasta", UserId = 1 };

        _mockMapper.Setup(m => m.Map<Recipe>(dto)).Returns(recipe);
        _mockRepo.Setup(r => r.AddAsync(recipe)).ReturnsAsync(recipe);
        _mockMapper.Setup(m => m.Map<RecipeDto>(recipe)).Returns(expected);

        var result = await _service.CreateRecipeAsync(dto);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenRecipeExists_ShouldReturnRecipeDto()
    {
        var recipe = new Recipe { Id = 1, Name = "Pasta", UserId = 1 };
        var expected = new RecipeDto { Id = 1, Name = "Pasta", UserId = 1 };

        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(recipe);
        _mockMapper.Setup(m => m.Map<RecipeDto>(recipe)).Returns(expected);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenRecipeNotFound_ShouldReturnNull()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Recipe?)null);

        var result = await _service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllByUserIdAsync_ShouldReturnMappedRecipes()
    {
        var recipes = new List<Recipe> { new() { Id = 1, Name = "Pasta", UserId = 1 } };
        var expected = new List<RecipeDto> { new() { Id = 1, Name = "Pasta", UserId = 1 } };

        _mockRepo.Setup(r => r.GetAllByUserIdAsync(1)).ReturnsAsync(recipes);
        _mockMapper.Setup(m => m.Map<IEnumerable<RecipeDto>>(recipes)).Returns(expected);

        var result = await _service.GetAllByUserIdAsync(1);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task UpdateRecipeAsync_WhenRecipeExists_ShouldReturnUpdatedDto()
    {
        var recipe = new Recipe { Id = 1, Name = "Pasta", UserId = 1 };
        var dto = new UpdateRecipeDto { Name = "Updated Pasta" };
        var expected = new RecipeDto { Id = 1, Name = "Updated Pasta", UserId = 1 };

        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(recipe);
        _mockMapper.Setup(m => m.Map<RecipeDto>(recipe)).Returns(expected);

        var result = await _service.UpdateRecipeAsync(1, dto);

        Assert.NotNull(result);
        Assert.Equal(expected, result);
        _mockRepo.Verify(r => r.UpdateAsync(recipe), Times.Once);
    }

    [Fact]
    public async Task UpdateRecipeAsync_WhenRecipeNotFound_ShouldReturnNull()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Recipe?)null);

        var result = await _service.UpdateRecipeAsync(999, new UpdateRecipeDto());

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteRecipeAsync_ShouldReturnRepoResult()
    {
        _mockRepo.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _service.DeleteRecipeAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task GetRotationSuggestionsAsync_ShouldReturnMappedRecipes()
    {
        var recipes = new List<Recipe> { new() { Id = 1, Name = "Old Recipe", UserId = 1 } };
        var expected = new List<RecipeDto> { new() { Id = 1, Name = "Old Recipe", UserId = 1 } };

        _mockRepo.Setup(r => r.GetLeastRecentlyCookedAsync(1, 5)).ReturnsAsync(recipes);
        _mockMapper.Setup(m => m.Map<IEnumerable<RecipeDto>>(recipes)).Returns(expected);

        var result = await _service.GetRotationSuggestionsAsync(1, 5);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task LogCookAsync_WhenRecipeExists_ShouldIncrementAndReturnDto()
    {
        var recipe = new Recipe { Id = 1, Name = "Pasta", CookCount = 2, LastCookedDate = null };
        var expected = new RecipeDto { Id = 1, Name = "Pasta", CookCount = 3 };

        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(recipe);
        _mockMapper.Setup(m => m.Map<RecipeDto>(recipe)).Returns(expected);

        var result = await _service.LogCookAsync(1);

        Assert.NotNull(result);
        Assert.Equal(3, recipe.CookCount);
        Assert.NotNull(recipe.LastCookedDate);
        _mockRepo.Verify(r => r.UpdateAsync(recipe), Times.Once);
    }

    [Fact]
    public async Task LogCookAsync_WhenRecipeNotFound_ShouldReturnNull()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Recipe?)null);

        var result = await _service.LogCookAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateRecipeAsync_ShouldClearExistingIngredients()
    {
        var recipe = new Recipe
        {
            Id = 1,
            Name = "Pasta",
            UserId = 1,
            Ingredients = new List<Ingredient>
            {
                new() { Id = 1, Name = "Old Ingredient", Quantity = 1 }
            }
        };
        var dto = new UpdateRecipeDto
        {
            Name = "Updated Pasta",
            Ingredients = new List<CreateIngredientDto>
            {
                new() { Name = "New Ingredient", Quantity = 2, Unit = "cups" }
            }
        };
        var expected = new RecipeDto { Id = 1, Name = "Updated Pasta", UserId = 1 };

        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(recipe);
        _mockMapper.Setup(m => m.Map<RecipeDto>(recipe)).Returns(expected);

        await _service.UpdateRecipeAsync(1, dto);

        Assert.Empty(recipe.Ingredients);
        _mockRepo.Verify(r => r.UpdateAsync(recipe), Times.Once);
    }
}

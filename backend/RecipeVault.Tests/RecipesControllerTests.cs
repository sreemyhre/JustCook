using Microsoft.AspNetCore.Mvc;
using Moq;
using RecipeVault.API.Controllers;
using RecipeVault.Application.DTOs;
using RecipeVault.Application.Interfaces;
using Xunit;

namespace RecipeVault.Tests;

public class RecipesControllerTests
{
    private readonly Mock<IRecipeService> _mockService;
    private readonly RecipesController _controller;

    public RecipesControllerTests()
    {
        _mockService = new Mock<IRecipeService>();
        _controller = new RecipesController(_mockService.Object);
    }

    [Fact]
    public async Task CreateRecipe_ShouldReturnCreatedAtAction()
    {
        var dto = new CreateRecipeDto { Name = "Pasta", UserId = 1 };
        var recipeDto = new RecipeDto { Id = 1, Name = "Pasta", UserId = 1 };

        _mockService.Setup(s => s.CreateRecipeAsync(dto)).ReturnsAsync(recipeDto);

        var result = await _controller.CreateRecipe(dto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, createdResult.StatusCode);
        Assert.Equal(recipeDto, createdResult.Value);
    }

    [Fact]
    public async Task CreateRecipe_ShouldCallServiceCreateRecipeAsync()
    {
        var dto = new CreateRecipeDto { Name = "Pasta", UserId = 1 };
        var recipeDto = new RecipeDto { Id = 1, Name = "Pasta", UserId = 1 };

        _mockService.Setup(s => s.CreateRecipeAsync(dto)).ReturnsAsync(recipeDto);

        await _controller.CreateRecipe(dto);

        _mockService.Verify(s => s.CreateRecipeAsync(dto), Times.Once);
    }

    [Fact]
    public async Task GetRecipe_WhenRecipeExists_ShouldReturnOk()
    {
        var recipeDto = new RecipeDto { Id = 1, Name = "Pasta", UserId = 1 };

        _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(recipeDto);

        var result = await _controller.GetRecipe(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(recipeDto, okResult.Value);
    }

    [Fact]
    public async Task GetRecipe_WhenRecipeNotFound_ShouldReturnNotFound()
    {
        _mockService.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((RecipeDto?)null);

        var result = await _controller.GetRecipe(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetAllRecipes_ShouldReturnOkWithRecipes()
    {
        var recipes = new List<RecipeDto> { new() { Id = 1, Name = "Pasta", UserId = 1 } };
        _mockService.Setup(s => s.GetAllByUserIdAsync(1)).ReturnsAsync(recipes);

        var result = await _controller.GetAllRecipes(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(recipes, okResult.Value);
    }

    [Fact]
    public async Task UpdateRecipe_WhenRecipeExists_ShouldReturnOk()
    {
        var dto = new UpdateRecipeDto { Name = "Updated" };
        var recipeDto = new RecipeDto { Id = 1, Name = "Updated", UserId = 1 };
        _mockService.Setup(s => s.UpdateRecipeAsync(1, dto)).ReturnsAsync(recipeDto);

        var result = await _controller.UpdateRecipe(1, dto);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(recipeDto, okResult.Value);
    }

    [Fact]
    public async Task UpdateRecipe_WhenRecipeNotFound_ShouldReturnNotFound()
    {
        _mockService.Setup(s => s.UpdateRecipeAsync(999, It.IsAny<UpdateRecipeDto>())).ReturnsAsync((RecipeDto?)null);

        var result = await _controller.UpdateRecipe(999, new UpdateRecipeDto());

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteRecipe_WhenDeleted_ShouldReturnNoContent()
    {
        _mockService.Setup(s => s.DeleteRecipeAsync(1)).ReturnsAsync(true);

        var result = await _controller.DeleteRecipe(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteRecipe_WhenNotFound_ShouldReturnNotFound()
    {
        _mockService.Setup(s => s.DeleteRecipeAsync(999)).ReturnsAsync(false);

        var result = await _controller.DeleteRecipe(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetRotationSuggestions_ShouldReturnOk()
    {
        var recipes = new List<RecipeDto> { new() { Id = 1, Name = "Old Recipe" } };
        _mockService.Setup(s => s.GetRotationSuggestionsAsync(1, 5)).ReturnsAsync(recipes);

        var result = await _controller.GetRotationSuggestions(1, 5);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(recipes, okResult.Value);
    }

    [Fact]
    public async Task LogCook_WhenRecipeExists_ShouldReturnOk()
    {
        var recipeDto = new RecipeDto { Id = 1, Name = "Pasta", CookCount = 3 };
        _mockService.Setup(s => s.LogCookAsync(1)).ReturnsAsync(recipeDto);

        var result = await _controller.LogCook(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(recipeDto, okResult.Value);
    }

    [Fact]
    public async Task LogCook_WhenRecipeNotFound_ShouldReturnNotFound()
    {
        _mockService.Setup(s => s.LogCookAsync(999)).ReturnsAsync((RecipeDto?)null);

        var result = await _controller.LogCook(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }
}

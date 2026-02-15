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
}

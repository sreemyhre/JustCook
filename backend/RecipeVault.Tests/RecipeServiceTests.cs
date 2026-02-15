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
}

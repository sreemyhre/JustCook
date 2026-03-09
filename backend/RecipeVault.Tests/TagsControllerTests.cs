using Microsoft.AspNetCore.Mvc;
using Moq;
using RecipeVault.API.Controllers;
using RecipeVault.Application.DTOs;
using RecipeVault.Application.Interfaces;
using Xunit;

namespace RecipeVault.Tests;

public class TagsControllerTests
{
    private readonly Mock<ITagService> _mockService;
    private readonly TagsController _controller;

    public TagsControllerTests()
    {
        _mockService = new Mock<ITagService>();
        _controller = new TagsController(_mockService.Object);
    }

    [Fact]
    public async Task CreateTag_ShouldReturnCreatedAtAction()
    {
        var dto = new CreateTagDto { Name = "Vegan" };
        var tagDto = new TagDto { Id = 1, Name = "Vegan" };

        _mockService.Setup(s => s.CreateTagAsync(dto)).ReturnsAsync(tagDto);

        var result = await _controller.CreateTag(dto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, createdResult.StatusCode);
        Assert.Equal(tagDto, createdResult.Value);
    }

    [Fact]
    public async Task GetTag_WhenTagExists_ShouldReturnOk()
    {
        var tagDto = new TagDto { Id = 1, Name = "Vegan" };

        _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(tagDto);

        var result = await _controller.GetTag(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(tagDto, okResult.Value);
    }

    [Fact]
    public async Task GetTag_WhenTagNotFound_ShouldReturnNotFound()
    {
        _mockService.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((TagDto?)null);

        var result = await _controller.GetTag(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetAllTags_ShouldReturnOkWithTags()
    {
        var tags = new List<TagDto> { new() { Id = 1, Name = "Vegan" } };
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(tags);

        var result = await _controller.GetAllTags();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(tags, okResult.Value);
    }

    [Fact]
    public async Task UpdateTag_WhenTagExists_ShouldReturnOk()
    {
        var dto = new UpdateTagDto { Name = "Plant-Based" };
        var tagDto = new TagDto { Id = 1, Name = "Plant-Based" };
        _mockService.Setup(s => s.UpdateTagAsync(1, dto)).ReturnsAsync(tagDto);

        var result = await _controller.UpdateTag(1, dto);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(tagDto, okResult.Value);
    }

    [Fact]
    public async Task UpdateTag_WhenTagNotFound_ShouldReturnNotFound()
    {
        _mockService.Setup(s => s.UpdateTagAsync(999, It.IsAny<UpdateTagDto>())).ReturnsAsync((TagDto?)null);

        var result = await _controller.UpdateTag(999, new UpdateTagDto());

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteTag_WhenDeleted_ShouldReturnNoContent()
    {
        _mockService.Setup(s => s.DeleteTagAsync(1)).ReturnsAsync(true);

        var result = await _controller.DeleteTag(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteTag_WhenNotFound_ShouldReturnNotFound()
    {
        _mockService.Setup(s => s.DeleteTagAsync(999)).ReturnsAsync(false);

        var result = await _controller.DeleteTag(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task AddTagToRecipe_WhenSuccess_ShouldReturnNoContent()
    {
        _mockService.Setup(s => s.AddTagToRecipeAsync(1, 2)).ReturnsAsync(true);

        var result = await _controller.AddTagToRecipe(2, 1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task AddTagToRecipe_WhenAlreadyAssigned_ShouldReturnConflict()
    {
        _mockService.Setup(s => s.AddTagToRecipeAsync(1, 2)).ReturnsAsync(false);

        var result = await _controller.AddTagToRecipe(2, 1);

        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task RemoveTagFromRecipe_WhenSuccess_ShouldReturnNoContent()
    {
        _mockService.Setup(s => s.RemoveTagFromRecipeAsync(1, 2)).ReturnsAsync(true);

        var result = await _controller.RemoveTagFromRecipe(2, 1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task RemoveTagFromRecipe_WhenNotFound_ShouldReturnNotFound()
    {
        _mockService.Setup(s => s.RemoveTagFromRecipeAsync(1, 2)).ReturnsAsync(false);

        var result = await _controller.RemoveTagFromRecipe(2, 1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetRecipesByTag_ShouldReturnOkWithRecipes()
    {
        var recipes = new List<RecipeDto> { new() { Id = 1, Name = "Salad" } };
        _mockService.Setup(s => s.GetRecipesByTagAsync(1)).ReturnsAsync(recipes);

        var result = await _controller.GetRecipesByTag(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(recipes, okResult.Value);
    }
}

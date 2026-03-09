using AutoMapper;
using Moq;
using RecipeVault.Application.DTOs;
using RecipeVault.Application.Services;
using RecipeVault.Core.Entities;
using RecipeVault.Core.Interfaces;
using Xunit;

namespace RecipeVault.Tests;

public class TagServiceTests
{
    private readonly Mock<ITagRepository> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly TagService _service;

    public TagServiceTests()
    {
        _mockRepo = new Mock<ITagRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new TagService(_mockRepo.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task CreateTagAsync_ShouldMapDtoToEntity()
    {
        var dto = new CreateTagDto { Name = "Vegan" };
        var tag = new Tag { Name = "Vegan" };
        var tagDto = new TagDto { Id = 1, Name = "Vegan" };

        _mockMapper.Setup(m => m.Map<Tag>(dto)).Returns(tag);
        _mockRepo.Setup(r => r.AddAsync(tag)).ReturnsAsync(tag);
        _mockMapper.Setup(m => m.Map<TagDto>(tag)).Returns(tagDto);

        await _service.CreateTagAsync(dto);

        _mockMapper.Verify(m => m.Map<Tag>(dto), Times.Once);
    }

    [Fact]
    public async Task CreateTagAsync_ShouldCallRepositoryAdd()
    {
        var dto = new CreateTagDto { Name = "Vegan" };
        var tag = new Tag { Name = "Vegan" };
        var tagDto = new TagDto { Id = 1, Name = "Vegan" };

        _mockMapper.Setup(m => m.Map<Tag>(dto)).Returns(tag);
        _mockRepo.Setup(r => r.AddAsync(tag)).ReturnsAsync(tag);
        _mockMapper.Setup(m => m.Map<TagDto>(tag)).Returns(tagDto);

        await _service.CreateTagAsync(dto);

        _mockRepo.Verify(r => r.AddAsync(tag), Times.Once);
    }

    [Fact]
    public async Task CreateTagAsync_ShouldReturnMappedTagDto()
    {
        var dto = new CreateTagDto { Name = "Vegan" };
        var tag = new Tag { Name = "Vegan" };
        var expected = new TagDto { Id = 1, Name = "Vegan" };

        _mockMapper.Setup(m => m.Map<Tag>(dto)).Returns(tag);
        _mockRepo.Setup(r => r.AddAsync(tag)).ReturnsAsync(tag);
        _mockMapper.Setup(m => m.Map<TagDto>(tag)).Returns(expected);

        var result = await _service.CreateTagAsync(dto);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenTagExists_ShouldReturnTagDto()
    {
        var tag = new Tag { Id = 1, Name = "Vegan" };
        var expected = new TagDto { Id = 1, Name = "Vegan" };

        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(tag);
        _mockMapper.Setup(m => m.Map<TagDto>(tag)).Returns(expected);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenTagNotFound_ShouldReturnNull()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Tag?)null);

        var result = await _service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedTags()
    {
        var tags = new List<Tag> { new() { Id = 1, Name = "Vegan" } };
        var expected = new List<TagDto> { new() { Id = 1, Name = "Vegan" } };

        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(tags);
        _mockMapper.Setup(m => m.Map<IEnumerable<TagDto>>(tags)).Returns(expected);

        var result = await _service.GetAllAsync();

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task UpdateTagAsync_WhenTagExists_ShouldReturnUpdatedDto()
    {
        var tag = new Tag { Id = 1, Name = "Vegan" };
        var dto = new UpdateTagDto { Name = "Plant-Based" };
        var expected = new TagDto { Id = 1, Name = "Plant-Based" };

        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(tag);
        _mockMapper.Setup(m => m.Map<TagDto>(tag)).Returns(expected);

        var result = await _service.UpdateTagAsync(1, dto);

        Assert.NotNull(result);
        Assert.Equal(expected, result);
        _mockRepo.Verify(r => r.UpdateAsync(tag), Times.Once);
    }

    [Fact]
    public async Task UpdateTagAsync_WhenTagNotFound_ShouldReturnNull()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Tag?)null);

        var result = await _service.UpdateTagAsync(999, new UpdateTagDto());

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteTagAsync_ShouldReturnRepoResult()
    {
        _mockRepo.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _service.DeleteTagAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task AddTagToRecipeAsync_ShouldReturnRepoResult()
    {
        _mockRepo.Setup(r => r.AddTagToRecipeAsync(1, 2)).ReturnsAsync(true);

        var result = await _service.AddTagToRecipeAsync(1, 2);

        Assert.True(result);
    }

    [Fact]
    public async Task RemoveTagFromRecipeAsync_ShouldReturnRepoResult()
    {
        _mockRepo.Setup(r => r.RemoveTagFromRecipeAsync(1, 2)).ReturnsAsync(true);

        var result = await _service.RemoveTagFromRecipeAsync(1, 2);

        Assert.True(result);
    }

    [Fact]
    public async Task GetRecipesByTagAsync_ShouldReturnMappedRecipes()
    {
        var recipes = new List<Recipe> { new() { Id = 1, Name = "Salad", UserId = 1 } };
        var expected = new List<RecipeDto> { new() { Id = 1, Name = "Salad", UserId = 1 } };

        _mockRepo.Setup(r => r.GetRecipesByTagIdAsync(1)).ReturnsAsync(recipes);
        _mockMapper.Setup(m => m.Map<IEnumerable<RecipeDto>>(recipes)).Returns(expected);

        var result = await _service.GetRecipesByTagAsync(1);

        Assert.Equal(expected, result);
    }
}

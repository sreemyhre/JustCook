using AutoMapper;
using Moq;
using RecipeVault.Application.DTOs;
using RecipeVault.Application.Services;
using RecipeVault.Core.Entities;
using RecipeVault.Core.Enums;
using RecipeVault.Core.Interfaces;
using Xunit;

namespace RecipeVault.Tests;

public class MealPlanServiceTests
{
    private readonly Mock<IMealPlanRepository> _mockRepo;
    private readonly Mock<IRecipeRepository> _mockRecipeRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly MealPlanService _service;

    public MealPlanServiceTests()
    {
        _mockRepo = new Mock<IMealPlanRepository>();
        _mockRecipeRepo = new Mock<IRecipeRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new MealPlanService(_mockRepo.Object, _mockRecipeRepo.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task CreateMealPlanAsync_ShouldCallRepositoryAdd()
    {
        var dto = new CreateMealPlanDto { UserId = 1, WeekStartDate = DateTime.Today };
        var mealPlan = new MealPlan { Id = 1, UserId = 1 };
        var mealPlanDto = new MealPlanDto { Id = 1, UserId = 1 };

        _mockMapper.Setup(m => m.Map<MealPlan>(dto)).Returns(mealPlan);
        _mockRepo.Setup(r => r.AddAsync(mealPlan)).ReturnsAsync(mealPlan);
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(mealPlan);
        _mockMapper.Setup(m => m.Map<MealPlanDto>(mealPlan)).Returns(mealPlanDto);

        await _service.CreateMealPlanAsync(dto);

        _mockRepo.Verify(r => r.AddAsync(mealPlan), Times.Once);
    }

    [Fact]
    public async Task CreateMealPlanAsync_ShouldReturnMappedDto()
    {
        var dto = new CreateMealPlanDto { UserId = 1, WeekStartDate = DateTime.Today };
        var mealPlan = new MealPlan { Id = 1, UserId = 1 };
        var expected = new MealPlanDto { Id = 1, UserId = 1 };

        _mockMapper.Setup(m => m.Map<MealPlan>(dto)).Returns(mealPlan);
        _mockRepo.Setup(r => r.AddAsync(mealPlan)).ReturnsAsync(mealPlan);
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(mealPlan);
        _mockMapper.Setup(m => m.Map<MealPlanDto>(mealPlan)).Returns(expected);

        var result = await _service.CreateMealPlanAsync(dto);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ShouldReturnDto()
    {
        var mealPlan = new MealPlan { Id = 1, UserId = 1 };
        var expected = new MealPlanDto { Id = 1, UserId = 1 };

        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(mealPlan);
        _mockMapper.Setup(m => m.Map<MealPlanDto>(mealPlan)).Returns(expected);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ShouldReturnNull()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((MealPlan?)null);

        var result = await _service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllByUserIdAsync_ShouldReturnMappedPlans()
    {
        var plans = new List<MealPlan> { new() { Id = 1, UserId = 1 } };
        var expected = new List<MealPlanDto> { new() { Id = 1, UserId = 1 } };

        _mockRepo.Setup(r => r.GetAllByUserIdAsync(1)).ReturnsAsync(plans);
        _mockMapper.Setup(m => m.Map<IEnumerable<MealPlanDto>>(plans)).Returns(expected);

        var result = await _service.GetAllByUserIdAsync(1);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task UpdateMealPlanAsync_WhenNotFound_ShouldReturnNull()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((MealPlan?)null);

        var result = await _service.UpdateMealPlanAsync(999, new CreateMealPlanDto());

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateMealPlanAsync_WhenExists_ShouldCallUpdate()
    {
        var mealPlan = new MealPlan { Id = 1, UserId = 1, Items = new List<MealPlanItem>() };
        var dto = new CreateMealPlanDto { UserId = 1, WeekStartDate = DateTime.Today };
        var expected = new MealPlanDto { Id = 1, UserId = 1 };

        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(mealPlan);
        _mockMapper.Setup(m => m.Map<MealPlanDto>(mealPlan)).Returns(expected);

        await _service.UpdateMealPlanAsync(1, dto);

        _mockRepo.Verify(r => r.UpdateAsync(mealPlan), Times.Once);
    }

    [Fact]
    public async Task DeleteMealPlanAsync_ShouldReturnRepoResult()
    {
        _mockRepo.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _service.DeleteMealPlanAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task GenerateMealPlanAsync_ShouldSelectRecipesAndSavePlan()
    {
        var recipes = Enumerable.Range(1, 10).Select(i => new Recipe
        {
            Id = i,
            UserId = 1,
            Name = $"Recipe {i}",
            CookCount = i,
            LastCookedDate = DateTime.UtcNow.AddDays(-i * 3),
            RecipeTags = new List<RecipeTag>()
        }).ToList();

        var dto = new GenerateMealPlanDto { UserId = 1, WeekStartDate = DateTime.Today };
        var savedPlan = new MealPlan { Id = 1, UserId = 1, Items = new List<MealPlanItem>() };
        var expected = new MealPlanDto { Id = 1, UserId = 1 };

        _mockRepo.Setup(r => r.GetUserRecipesWithTagsAsync(1)).ReturnsAsync(recipes);
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<MealPlan>())).ReturnsAsync(savedPlan);
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(savedPlan);
        _mockMapper.Setup(m => m.Map<MealPlanDto>(savedPlan)).Returns(expected);

        var result = await _service.GenerateMealPlanAsync(dto);

        _mockRepo.Verify(r => r.AddAsync(It.Is<MealPlan>(p => p.Items.Count == 7)), Times.Once);
        _mockRecipeRepo.Verify(
            r => r.UpdateAsync(It.Is<Recipe>(rec => rec.LastPlannedDate.HasValue)),
            Times.Exactly(7));
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task GenerateMealPlanAsync_WithTagQuotas_ShouldPrioritiseTaggedRecipes()
    {
        var tag1 = new Tag { Id = 1, Name = "Vegan" };
        var recipes = new List<Recipe>
        {
            new() { Id = 1, UserId = 1, Name = "Vegan Curry", CookCount = 0, RecipeTags = new List<RecipeTag> { new() { TagId = 1, Tag = tag1 } } },
            new() { Id = 2, UserId = 1, Name = "Vegan Dal", CookCount = 0, RecipeTags = new List<RecipeTag> { new() { TagId = 1, Tag = tag1 } } },
            new() { Id = 3, UserId = 1, Name = "Rice", CookCount = 0, RecipeTags = new List<RecipeTag>() },
            new() { Id = 4, UserId = 1, Name = "Pasta", CookCount = 0, RecipeTags = new List<RecipeTag>() },
            new() { Id = 5, UserId = 1, Name = "Soup", CookCount = 0, RecipeTags = new List<RecipeTag>() },
            new() { Id = 6, UserId = 1, Name = "Stew", CookCount = 0, RecipeTags = new List<RecipeTag>() },
            new() { Id = 7, UserId = 1, Name = "Salad", CookCount = 0, RecipeTags = new List<RecipeTag>() },
        };

        var dto = new GenerateMealPlanDto
        {
            UserId = 1,
            WeekStartDate = DateTime.Today,
            TagQuotas = new List<TagQuotaDto> { new() { TagId = 1, Count = 2 } }
        };

        var savedPlan = new MealPlan { Id = 1, UserId = 1, Items = new List<MealPlanItem>() };
        var expected = new MealPlanDto { Id = 1, UserId = 1 };

        _mockRepo.Setup(r => r.GetUserRecipesWithTagsAsync(1)).ReturnsAsync(recipes);
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<MealPlan>())).ReturnsAsync(savedPlan);
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(savedPlan);
        _mockMapper.Setup(m => m.Map<MealPlanDto>(savedPlan)).Returns(expected);

        await _service.GenerateMealPlanAsync(dto);

        _mockRepo.Verify(r => r.AddAsync(It.Is<MealPlan>(p =>
            p.Items.Count == 7 &&
            p.Items.Any(i => i.RecipeId == 1) &&
            p.Items.Any(i => i.RecipeId == 2)
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateMealPlanAsync_RemovedRecipe_ShouldClearLastPlannedDate()
    {
        var removedRecipe = new Recipe { Id = 10, LastPlannedDate = DateTime.UtcNow.AddDays(-1) };
        var mealPlan = new MealPlan
        {
            Id = 1, UserId = 1, WeekStartDate = DateTime.Today,
            Items = new List<MealPlanItem>
            {
                new() { RecipeId = 10, DayOfWeek = DayOfWeekEnum.Monday, Recipe = removedRecipe }
            }
        };
        var dto = new CreateMealPlanDto
        {
            UserId = 1, WeekStartDate = DateTime.Today,
            Items = new List<CreateMealPlanItemDto>
            {
                new() { RecipeId = 20, DayOfWeek = DayOfWeekEnum.Monday }
            }
        };
        var newRecipe = new Recipe { Id = 20 };

        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(mealPlan);
        _mockRecipeRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(removedRecipe);
        _mockRecipeRepo.Setup(r => r.GetByIdAsync(20)).ReturnsAsync(newRecipe);

        await _service.UpdateMealPlanAsync(1, dto);

        _mockRecipeRepo.Verify(
            r => r.UpdateAsync(It.Is<Recipe>(rec => rec.Id == 10 && rec.LastPlannedDate == null)),
            Times.Once);
    }

    [Fact]
    public async Task UpdateMealPlanAsync_AddedRecipe_ShouldStampLastPlannedDate()
    {
        var mealPlan = new MealPlan
        {
            Id = 1, UserId = 1, WeekStartDate = DateTime.Today,
            Items = new List<MealPlanItem>()
        };
        var dto = new CreateMealPlanDto
        {
            UserId = 1, WeekStartDate = DateTime.Today,
            Items = new List<CreateMealPlanItemDto>
            {
                new() { RecipeId = 5, DayOfWeek = DayOfWeekEnum.Wednesday }
            }
        };
        var newRecipe = new Recipe { Id = 5 };

        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(mealPlan);
        _mockRecipeRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(newRecipe);

        await _service.UpdateMealPlanAsync(1, dto);

        var expectedDate = DateTime.Today.AddDays((int)DayOfWeekEnum.Wednesday);
        _mockRecipeRepo.Verify(
            r => r.UpdateAsync(It.Is<Recipe>(rec => rec.Id == 5 && rec.LastPlannedDate == expectedDate)),
            Times.Once);
    }

    [Fact]
    public async Task UpdateMealPlanAsync_UnchangedRecipe_ShouldNotUpdateLastPlannedDate()
    {
        var existingRecipe = new Recipe { Id = 7, LastPlannedDate = DateTime.Today.AddDays(-7) };
        var mealPlan = new MealPlan
        {
            Id = 1, UserId = 1, WeekStartDate = DateTime.Today,
            Items = new List<MealPlanItem>
            {
                new() { RecipeId = 7, DayOfWeek = DayOfWeekEnum.Friday, Recipe = existingRecipe }
            }
        };
        var dto = new CreateMealPlanDto
        {
            UserId = 1, WeekStartDate = DateTime.Today,
            Items = new List<CreateMealPlanItemDto>
            {
                new() { RecipeId = 7, DayOfWeek = DayOfWeekEnum.Friday }
            }
        };

        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(mealPlan);

        await _service.UpdateMealPlanAsync(1, dto);

        _mockRecipeRepo.Verify(r => r.UpdateAsync(It.Is<Recipe>(rec => rec.Id == 7)), Times.Never);
    }
}

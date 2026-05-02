using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RecipeVault.API.Controllers;
using RecipeVault.Application.DTOs;
using RecipeVault.Application.Interfaces;
using Xunit;

namespace RecipeVault.Tests;

public class MealPlansControllerTests
{
    private readonly Mock<IMealPlanService> _mockService;
    private readonly MealPlansController _controller;

    public MealPlansControllerTests()
    {
        _mockService = new Mock<IMealPlanService>();
        _controller = new MealPlansController(_mockService.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("sub", "1")
        }, "TestAuth"));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task CreateMealPlan_ShouldReturnCreatedAtAction()
    {
        var dto = new CreateMealPlanDto { UserId = 1, WeekStartDate = DateTime.Today };
        var planDto = new MealPlanDto { Id = 1, UserId = 1 };

        _mockService.Setup(s => s.CreateMealPlanAsync(dto)).ReturnsAsync(planDto);

        var result = await _controller.CreateMealPlan(dto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, createdResult.StatusCode);
        Assert.Equal(planDto, createdResult.Value);
    }

    [Fact]
    public async Task GetMealPlan_WhenExists_ShouldReturnOk()
    {
        var planDto = new MealPlanDto { Id = 1, UserId = 1 };

        _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(planDto);

        var result = await _controller.GetMealPlan(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(planDto, okResult.Value);
    }

    [Fact]
    public async Task GetMealPlan_WhenNotFound_ShouldReturnNotFound()
    {
        _mockService.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((MealPlanDto?)null);

        var result = await _controller.GetMealPlan(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetAllMealPlans_ShouldReturnOkWithPlans()
    {
        var plans = new List<MealPlanDto> { new() { Id = 1, UserId = 1 } };
        _mockService.Setup(s => s.GetAllByUserIdAsync(1)).ReturnsAsync(plans);

        var result = await _controller.GetAllMealPlans();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(plans, okResult.Value);
    }

    [Fact]
    public async Task UpdateMealPlan_WhenExists_ShouldReturnOk()
    {
        var dto = new CreateMealPlanDto { UserId = 1, WeekStartDate = DateTime.Today };
        var planDto = new MealPlanDto { Id = 1, UserId = 1 };
        _mockService.Setup(s => s.UpdateMealPlanAsync(1, dto)).ReturnsAsync(planDto);

        var result = await _controller.UpdateMealPlan(1, dto);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(planDto, okResult.Value);
    }

    [Fact]
    public async Task UpdateMealPlan_WhenNotFound_ShouldReturnNotFound()
    {
        _mockService.Setup(s => s.UpdateMealPlanAsync(999, It.IsAny<CreateMealPlanDto>())).ReturnsAsync((MealPlanDto?)null);

        var result = await _controller.UpdateMealPlan(999, new CreateMealPlanDto());

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteMealPlan_WhenDeleted_ShouldReturnNoContent()
    {
        _mockService.Setup(s => s.DeleteMealPlanAsync(1)).ReturnsAsync(true);

        var result = await _controller.DeleteMealPlan(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteMealPlan_WhenNotFound_ShouldReturnNotFound()
    {
        _mockService.Setup(s => s.DeleteMealPlanAsync(999)).ReturnsAsync(false);

        var result = await _controller.DeleteMealPlan(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GenerateMealPlan_ShouldReturnCreatedAtAction()
    {
        var dto = new GenerateMealPlanDto { UserId = 1, WeekStartDate = DateTime.Today };
        var planDto = new MealPlanDto { Id = 1, UserId = 1 };

        _mockService.Setup(s => s.GenerateMealPlanAsync(dto)).ReturnsAsync(planDto);

        var result = await _controller.GenerateMealPlan(dto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, createdResult.StatusCode);
        Assert.Equal(planDto, createdResult.Value);
    }
}

using Microsoft.AspNetCore.Mvc;
using RecipeVault.Application.DTOs;
using RecipeVault.Application.Interfaces;

namespace RecipeVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MealPlansController : ControllerBase
{
    private readonly IMealPlanService _mealPlanService;

    public MealPlansController(IMealPlanService mealPlanService)
    {
        _mealPlanService = mealPlanService;
    }

    [HttpPost]
    public async Task<ActionResult<MealPlanDto>> CreateMealPlan(CreateMealPlanDto dto)
    {
        var plan = await _mealPlanService.CreateMealPlanAsync(dto);
        return CreatedAtAction(nameof(GetMealPlan), new { id = plan.Id }, plan);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MealPlanDto>> GetMealPlan(int id)
    {
        var plan = await _mealPlanService.GetByIdAsync(id);
        if (plan == null) return NotFound();
        return Ok(plan);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MealPlanDto>>> GetAllMealPlans([FromQuery] int userId)
    {
        var plans = await _mealPlanService.GetAllByUserIdAsync(userId);
        return Ok(plans);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MealPlanDto>> UpdateMealPlan(int id, CreateMealPlanDto dto)
    {
        var plan = await _mealPlanService.UpdateMealPlanAsync(id, dto);
        if (plan == null) return NotFound();
        return Ok(plan);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMealPlan(int id)
    {
        var deleted = await _mealPlanService.DeleteMealPlanAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpPost("generate")]
    public async Task<ActionResult<MealPlanDto>> GenerateMealPlan(GenerateMealPlanDto dto)
    {
        var plan = await _mealPlanService.GenerateMealPlanAsync(dto);
        return CreatedAtAction(nameof(GetMealPlan), new { id = plan.Id }, plan);
    }
}

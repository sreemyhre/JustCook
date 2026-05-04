using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeVault.Application.DTOs;
using RecipeVault.Application.Interfaces;

namespace RecipeVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RecipesController : ControllerBase
{
    private readonly IRecipeService _recipeService;

    public RecipesController(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    private int GetUserId()
    {
        var sub = User.FindFirst("sub")?.Value
            ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(sub, out var id) ? id : 0;
    }

    [HttpPost]
    public async Task<ActionResult<RecipeDto>> CreateRecipe(CreateRecipeDto dto)
    {
        dto.UserId = GetUserId();
        var recipe = await _recipeService.CreateRecipeAsync(dto);
        return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, recipe);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RecipeDto>> GetRecipe(int id)
    {
        var recipe = await _recipeService.GetByIdAsync(id, GetUserId());
        if (recipe == null) return NotFound();
        return Ok(recipe);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RecipeDto>>> GetAllRecipes()
    {
        var recipes = await _recipeService.GetAllByUserIdAsync(GetUserId());
        return Ok(recipes);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<RecipeDto>> UpdateRecipe(int id, UpdateRecipeDto dto)
    {
        var recipe = await _recipeService.UpdateRecipeAsync(id, GetUserId(), dto);
        if (recipe == null) return NotFound();
        return Ok(recipe);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecipe(int id)
    {
        var deleted = await _recipeService.DeleteRecipeAsync(id, GetUserId());
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpGet("rotation-suggestions")]
    public async Task<ActionResult<IEnumerable<RecipeDto>>> GetRotationSuggestions([FromQuery] int count = 5)
    {
        var recipes = await _recipeService.GetRotationSuggestionsAsync(GetUserId(), count);
        return Ok(recipes);
    }

    [HttpPatch("{id}/log-cook")]
    public async Task<ActionResult<RecipeDto>> LogCook(int id)
    {
        var recipe = await _recipeService.LogCookAsync(id, GetUserId());
        if (recipe == null) return NotFound();
        return Ok(recipe);
    }
}

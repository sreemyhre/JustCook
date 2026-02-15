using Microsoft.AspNetCore.Mvc;
using RecipeVault.Application.DTOs;
using RecipeVault.Application.Interfaces;

namespace RecipeVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipesController : ControllerBase
{
    private readonly IRecipeService _recipeService;

    public RecipesController(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    [HttpPost]
    public async Task<ActionResult<RecipeDto>> CreateRecipe(CreateRecipeDto dto)
    {
        var recipe = await _recipeService.CreateRecipeAsync(dto);
        return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, recipe);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RecipeDto>> GetRecipe(int id)
    {
        var recipe = await _recipeService.GetByIdAsync(id);
        if (recipe == null) return NotFound();
        return Ok(recipe);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RecipeDto>>> GetAllRecipes([FromQuery] int userId)
    {
        var recipes = await _recipeService.GetAllByUserIdAsync(userId);
        return Ok(recipes);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<RecipeDto>> UpdateRecipe(int id, UpdateRecipeDto dto)
    {
        var recipe = await _recipeService.UpdateRecipeAsync(id, dto);
        if (recipe == null) return NotFound();
        return Ok(recipe);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecipe(int id)
    {
        var deleted = await _recipeService.DeleteRecipeAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpGet("rotation-suggestions")]
    public async Task<ActionResult<IEnumerable<RecipeDto>>> GetRotationSuggestions([FromQuery] int userId, [FromQuery] int count = 5)
    {
        var recipes = await _recipeService.GetRotationSuggestionsAsync(userId, count);
        return Ok(recipes);
    }

    [HttpPatch("{id}/log-cook")]
    public async Task<ActionResult<RecipeDto>> LogCook(int id)
    {
        var recipe = await _recipeService.LogCookAsync(id);
        if (recipe == null) return NotFound();
        return Ok(recipe);
    }
}

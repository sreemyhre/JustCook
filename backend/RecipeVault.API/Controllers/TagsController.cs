using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeVault.Application.DTOs;
using RecipeVault.Application.Interfaces;

namespace RecipeVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TagsController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagsController(ITagService tagService)
    {
        _tagService = tagService;
    }

    private int GetUserId()
    {
        var sub = User.FindFirst("sub")?.Value
            ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(sub, out var id) ? id : 0;
    }

    [HttpPost]
    public async Task<ActionResult<TagDto>> CreateTag(CreateTagDto dto)
    {
        dto.UserId = GetUserId();
        var tag = await _tagService.CreateTagAsync(dto);
        return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, tag);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TagDto>> GetTag(int id)
    {
        var tag = await _tagService.GetByIdAsync(id, GetUserId());
        if (tag == null) return NotFound();
        return Ok(tag);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TagDto>>> GetAllTags()
    {
        var tags = await _tagService.GetAllByUserIdAsync(GetUserId());
        return Ok(tags);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TagDto>> UpdateTag(int id, UpdateTagDto dto)
    {
        var tag = await _tagService.UpdateTagAsync(id, GetUserId(), dto);
        if (tag == null) return NotFound();
        return Ok(tag);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTag(int id)
    {
        var deleted = await _tagService.DeleteTagAsync(id, GetUserId());
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpPost("{id}/recipes/{recipeId}")]
    public async Task<IActionResult> AddTagToRecipe(int id, int recipeId)
    {
        var result = await _tagService.AddTagToRecipeAsync(recipeId, id);
        if (!result) return Conflict("Tag is already assigned to this recipe.");
        return NoContent();
    }

    [HttpDelete("{id}/recipes/{recipeId}")]
    public async Task<IActionResult> RemoveTagFromRecipe(int id, int recipeId)
    {
        var result = await _tagService.RemoveTagFromRecipeAsync(recipeId, id);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpGet("{id}/recipes")]
    public async Task<ActionResult<IEnumerable<RecipeDto>>> GetRecipesByTag(int id)
    {
        var recipes = await _tagService.GetRecipesByTagAsync(id, GetUserId());
        return Ok(recipes);
    }
}

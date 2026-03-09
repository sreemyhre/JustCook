using RecipeVault.Application.DTOs;

namespace RecipeVault.Application.Interfaces;

public interface ITagService
{
    Task<TagDto> CreateTagAsync(CreateTagDto dto);
    Task<TagDto?> GetByIdAsync(int id);
    Task<IEnumerable<TagDto>> GetAllAsync();
    Task<TagDto?> UpdateTagAsync(int id, UpdateTagDto dto);
    Task<bool> DeleteTagAsync(int id);
    Task<bool> AddTagToRecipeAsync(int recipeId, int tagId);
    Task<bool> RemoveTagFromRecipeAsync(int recipeId, int tagId);
    Task<IEnumerable<RecipeDto>> GetRecipesByTagAsync(int tagId);
}

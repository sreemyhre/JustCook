using RecipeVault.Application.DTOs;

namespace RecipeVault.Application.Interfaces;

public interface ITagService
{
    Task<TagDto> CreateTagAsync(CreateTagDto dto);
    Task<TagDto?> GetByIdAsync(int id, int userId);
    Task<IEnumerable<TagDto>> GetAllByUserIdAsync(int userId);
    Task<TagDto?> UpdateTagAsync(int id, int userId, UpdateTagDto dto);
    Task<bool> DeleteTagAsync(int id, int userId);
    Task<bool> AddTagToRecipeAsync(int recipeId, int tagId);
    Task<bool> RemoveTagFromRecipeAsync(int recipeId, int tagId);
    Task<IEnumerable<RecipeDto>> GetRecipesByTagAsync(int tagId, int userId);
}

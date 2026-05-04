using RecipeVault.Core.Entities;

namespace RecipeVault.Core.Interfaces;

public interface ITagRepository
{
    Task<Tag?> GetByIdAsync(int id, int userId);
    Task<IEnumerable<Tag>> GetAllByUserIdAsync(int userId);
    Task<IEnumerable<Recipe>> GetRecipesByTagIdAsync(int tagId, int userId);
    Task<Tag> AddAsync(Tag tag);
    Task UpdateAsync(Tag tag);
    Task<bool> DeleteAsync(int id, int userId);
    Task<bool> AddTagToRecipeAsync(int recipeId, int tagId);
    Task<bool> RemoveTagFromRecipeAsync(int recipeId, int tagId);
}

using RecipeVault.Core.Entities;

namespace RecipeVault.Core.Interfaces;

public interface ITagRepository
{
    Task<Tag?> GetByIdAsync(int id);
    Task<IEnumerable<Tag>> GetAllAsync();
    Task<IEnumerable<Recipe>> GetRecipesByTagIdAsync(int tagId);
    Task<Tag> AddAsync(Tag tag);
    Task UpdateAsync(Tag tag);
    Task<bool> DeleteAsync(int id);
    Task<bool> AddTagToRecipeAsync(int recipeId, int tagId);
    Task<bool> RemoveTagFromRecipeAsync(int recipeId, int tagId);
}

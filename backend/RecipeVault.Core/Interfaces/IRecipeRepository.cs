using RecipeVault.Core.Entities;

namespace RecipeVault.Core.Interfaces;

public interface IRecipeRepository
{
    Task<Recipe?> GetByIdAsync(int id);
    Task<IEnumerable<Recipe>> GetAllByUserIdAsync(int userId);
    Task<Recipe> AddAsync(Recipe recipe);
    Task UpdateAsync(Recipe recipe);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<Recipe>> GetLeastRecentlyCookedAsync(int userId, int count);
}

using RecipeVault.Core.Entities;

namespace RecipeVault.Core.Interfaces;

public interface IRecipeRepository
{
    Task<Recipe?> GetByIdAsync(int id);
    Task<Recipe> AddAsync(Recipe recipe);
}

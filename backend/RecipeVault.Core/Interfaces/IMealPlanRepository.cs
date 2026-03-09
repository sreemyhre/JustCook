using RecipeVault.Core.Entities;

namespace RecipeVault.Core.Interfaces;

public interface IMealPlanRepository
{
    Task<MealPlan?> GetByIdAsync(int id);
    Task<IEnumerable<MealPlan>> GetAllByUserIdAsync(int userId);
    Task<MealPlan> AddAsync(MealPlan mealPlan);
    Task UpdateAsync(MealPlan mealPlan);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<Recipe>> GetUserRecipesWithTagsAsync(int userId);
}

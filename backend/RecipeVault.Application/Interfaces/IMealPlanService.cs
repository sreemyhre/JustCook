using RecipeVault.Application.DTOs;

namespace RecipeVault.Application.Interfaces;

public interface IMealPlanService
{
    Task<MealPlanDto> CreateMealPlanAsync(CreateMealPlanDto dto);
    Task<MealPlanDto?> GetByIdAsync(int id);
    Task<IEnumerable<MealPlanDto>> GetAllByUserIdAsync(int userId);
    Task<MealPlanDto?> UpdateMealPlanAsync(int id, CreateMealPlanDto dto);
    Task<bool> DeleteMealPlanAsync(int id);
    Task<MealPlanDto> GenerateMealPlanAsync(GenerateMealPlanDto dto);
    Task<MealPlanDto> PreviewMealPlanAsync(GenerateMealPlanDto dto);
}

using RecipeVault.Application.DTOs;

namespace RecipeVault.Application.Interfaces;

public interface IRecipeService
{
    Task<RecipeDto> CreateRecipeAsync(CreateRecipeDto dto);
    Task<RecipeDto?> GetByIdAsync(int id);
    Task<IEnumerable<RecipeDto>> GetAllByUserIdAsync(int userId);
    Task<RecipeDto?> UpdateRecipeAsync(int id, UpdateRecipeDto dto);
    Task<bool> DeleteRecipeAsync(int id);
    Task<IEnumerable<RecipeDto>> GetRotationSuggestionsAsync(int userId, int count);
    Task<RecipeDto?> LogCookAsync(int id);
}

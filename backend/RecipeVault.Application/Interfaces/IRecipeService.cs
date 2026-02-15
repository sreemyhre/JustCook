using RecipeVault.Application.DTOs;

namespace RecipeVault.Application.Interfaces;

public interface IRecipeService
{
    Task<RecipeDto> CreateRecipeAsync(CreateRecipeDto dto);
    Task<RecipeDto?> GetByIdAsync(int id);
}

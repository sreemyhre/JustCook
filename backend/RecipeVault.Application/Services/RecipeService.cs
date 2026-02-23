using AutoMapper;
using RecipeVault.Application.DTOs;
using RecipeVault.Application.Interfaces;
using RecipeVault.Core.Entities;
using RecipeVault.Core.Interfaces;

namespace RecipeVault.Application.Services;

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IMapper _mapper;

    public RecipeService(IRecipeRepository recipeRepository, IMapper mapper)
    {
        _recipeRepository = recipeRepository;
        _mapper = mapper;
    }

    public async Task<RecipeDto> CreateRecipeAsync(CreateRecipeDto dto)
    {
        var recipe = _mapper.Map<Recipe>(dto);
        var created = await _recipeRepository.AddAsync(recipe);
        return _mapper.Map<RecipeDto>(created);
    }

    public async Task<RecipeDto?> GetByIdAsync(int id)
    {
        var recipe = await _recipeRepository.GetByIdAsync(id);
        if (recipe == null) return null;
        return _mapper.Map<RecipeDto>(recipe);
    }

    public async Task<IEnumerable<RecipeDto>> GetAllByUserIdAsync(int userId)
    {
        var recipes = await _recipeRepository.GetAllByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<RecipeDto>>(recipes);
    }

    public async Task<RecipeDto?> UpdateRecipeAsync(int id, UpdateRecipeDto dto)
    {
        var recipe = await _recipeRepository.GetByIdAsync(id);
        if (recipe == null) return null;

        recipe.Ingredients.Clear();
        _mapper.Map(dto, recipe);
        await _recipeRepository.UpdateAsync(recipe);
        return _mapper.Map<RecipeDto>(recipe);
    }

    public async Task<bool> DeleteRecipeAsync(int id)
    {
        return await _recipeRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<RecipeDto>> GetRotationSuggestionsAsync(int userId, int count)
    {
        var recipes = await _recipeRepository.GetLeastRecentlyCookedAsync(userId, count);
        return _mapper.Map<IEnumerable<RecipeDto>>(recipes);
    }

    public async Task<RecipeDto?> LogCookAsync(int id)
    {
        var recipe = await _recipeRepository.GetByIdAsync(id);
        if (recipe == null) return null;

        recipe.CookCount++;
        recipe.LastCookedDate = DateTime.UtcNow;
        await _recipeRepository.UpdateAsync(recipe);
        return _mapper.Map<RecipeDto>(recipe);
    }
}

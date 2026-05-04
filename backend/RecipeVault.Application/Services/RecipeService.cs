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
        await _recipeRepository.SetTagsAsync(created.Id, dto.TagIds);
        var result = await _recipeRepository.GetByIdAsync(created.Id);
        return _mapper.Map<RecipeDto>(result!);
    }

    public async Task<RecipeDto?> GetByIdAsync(int id, int userId)
    {
        var recipe = await _recipeRepository.GetByIdAsync(id);
        if (recipe == null || recipe.UserId != userId) return null;
        return _mapper.Map<RecipeDto>(recipe);
    }

    public async Task<IEnumerable<RecipeDto>> GetAllByUserIdAsync(int userId)
    {
        var recipes = await _recipeRepository.GetAllByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<RecipeDto>>(recipes);
    }

    public async Task<RecipeDto?> UpdateRecipeAsync(int id, int userId, UpdateRecipeDto dto)
    {
        var recipe = await _recipeRepository.GetByIdAsync(id);
        if (recipe == null || recipe.UserId != userId) return null;

        recipe.Ingredients.Clear();
        _mapper.Map(dto, recipe);
        await _recipeRepository.UpdateAsync(recipe);
        await _recipeRepository.SetTagsAsync(id, dto.TagIds);
        var result = await _recipeRepository.GetByIdAsync(id);
        return _mapper.Map<RecipeDto>(result!);
    }

    public async Task<bool> DeleteRecipeAsync(int id, int userId)
    {
        var recipe = await _recipeRepository.GetByIdAsync(id);
        if (recipe == null || recipe.UserId != userId) return false;
        return await _recipeRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<RecipeDto>> GetRotationSuggestionsAsync(int userId, int count)
    {
        var recipes = await _recipeRepository.GetLeastRecentlyCookedAsync(userId, count);
        return _mapper.Map<IEnumerable<RecipeDto>>(recipes);
    }

    public async Task<RecipeDto?> LogCookAsync(int id, int userId)
    {
        var recipe = await _recipeRepository.GetByIdAsync(id);
        if (recipe == null || recipe.UserId != userId) return null;

        recipe.CookCount++;
        recipe.LastCookedDate = DateTime.UtcNow;
        await _recipeRepository.UpdateAsync(recipe);
        return _mapper.Map<RecipeDto>(recipe);
    }
}

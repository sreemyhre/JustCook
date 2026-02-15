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
}

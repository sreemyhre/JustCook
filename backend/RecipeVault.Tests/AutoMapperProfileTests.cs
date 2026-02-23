using AutoMapper;
using RecipeVault.Application.DTOs;
using RecipeVault.Application.Mappings;
using RecipeVault.Core.Entities;
using Xunit;

namespace RecipeVault.Tests;

public class AutoMapperProfileTests
{
    private readonly IMapper _mapper;

    public AutoMapperProfileTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void MapperConfiguration_ShouldNotThrowOnMapping()
    {
        var createDto = new CreateRecipeDto { Name = "Test", UserId = 1 };
        var updateDto = new UpdateRecipeDto { Name = "Test" };
        var recipe = new Recipe { Id = 1, Name = "Test", UserId = 1 };
        var createIngredientDto = new CreateIngredientDto { Name = "Salt", Quantity = 1 };
        var ingredient = new Ingredient { Id = 1, RecipeId = 1, Name = "Salt", Quantity = 1 };

        var ex1 = Record.Exception(() => _mapper.Map<Recipe>(createDto));
        var ex2 = Record.Exception(() => _mapper.Map<Recipe>(updateDto));
        var ex3 = Record.Exception(() => _mapper.Map<RecipeDto>(recipe));
        var ex4 = Record.Exception(() => _mapper.Map<Ingredient>(createIngredientDto));
        var ex5 = Record.Exception(() => _mapper.Map<IngredientDto>(ingredient));

        Assert.Null(ex1);
        Assert.Null(ex2);
        Assert.Null(ex3);
        Assert.Null(ex4);
        Assert.Null(ex5);
    }

    [Fact]
    public void CreateRecipeDto_ShouldMapTo_Recipe()
    {
        var dto = new CreateRecipeDto
        {
            UserId = 1,
            Name = "Pasta",
            Description = "Delicious pasta",
            ServingSize = 4,
            PrepTimeMinutes = 10,
            CookTimeMinutes = 20,
            ImageUrl = "http://example.com/pasta.jpg"
        };

        var recipe = _mapper.Map<Recipe>(dto);

        Assert.Equal(dto.UserId, recipe.UserId);
        Assert.Equal(dto.Name, recipe.Name);
        Assert.Equal(dto.Description, recipe.Description);
        Assert.Equal(dto.ServingSize, recipe.ServingSize);
        Assert.Equal(dto.PrepTimeMinutes, recipe.PrepTimeMinutes);
        Assert.Equal(dto.CookTimeMinutes, recipe.CookTimeMinutes);
        Assert.Equal(dto.ImageUrl, recipe.ImageUrl);
    }

    [Fact]
    public void Recipe_ShouldMapTo_RecipeDto()
    {
        var recipe = new Recipe
        {
            Id = 1,
            UserId = 2,
            Name = "Tacos",
            Description = "Crunchy tacos",
            ServingSize = 3,
            PrepTimeMinutes = 15,
            CookTimeMinutes = 10,
            LastCookedDate = new DateTime(2025, 1, 1),
            CookCount = 5,
            ImageUrl = "http://example.com/tacos.jpg",
            CreatedAt = new DateTime(2024, 6, 1)
        };

        var dto = _mapper.Map<RecipeDto>(recipe);

        Assert.Equal(recipe.Id, dto.Id);
        Assert.Equal(recipe.UserId, dto.UserId);
        Assert.Equal(recipe.Name, dto.Name);
        Assert.Equal(recipe.Description, dto.Description);
        Assert.Equal(recipe.ServingSize, dto.ServingSize);
        Assert.Equal(recipe.PrepTimeMinutes, dto.PrepTimeMinutes);
        Assert.Equal(recipe.CookTimeMinutes, dto.CookTimeMinutes);
        Assert.Equal(recipe.LastCookedDate, dto.LastCookedDate);
        Assert.Equal(recipe.CookCount, dto.CookCount);
        Assert.Equal(recipe.ImageUrl, dto.ImageUrl);
        Assert.Equal(recipe.CreatedAt, dto.CreatedAt);
    }

    [Fact]
    public void CreateIngredientDto_ShouldMapTo_Ingredient()
    {
        var dto = new CreateIngredientDto
        {
            Name = "Spaghetti",
            Quantity = 400,
            Unit = "g",
            IsStaple = false
        };

        var ingredient = _mapper.Map<Ingredient>(dto);

        Assert.Equal(dto.Name, ingredient.Name);
        Assert.Equal(dto.Quantity, ingredient.Quantity);
        Assert.Equal(dto.Unit, ingredient.Unit);
        Assert.Equal(dto.IsStaple, ingredient.IsStaple);
    }

    [Fact]
    public void Ingredient_ShouldMapTo_IngredientDto()
    {
        var ingredient = new Ingredient
        {
            Id = 1,
            RecipeId = 5,
            Name = "Eggs",
            Quantity = 3,
            Unit = null,
            IsStaple = true
        };

        var dto = _mapper.Map<IngredientDto>(ingredient);

        Assert.Equal(ingredient.Id, dto.Id);
        Assert.Equal(ingredient.RecipeId, dto.RecipeId);
        Assert.Equal(ingredient.Name, dto.Name);
        Assert.Equal(ingredient.Quantity, dto.Quantity);
        Assert.Equal(ingredient.Unit, dto.Unit);
        Assert.Equal(ingredient.IsStaple, dto.IsStaple);
    }

    [Fact]
    public void CreateRecipeDto_WithIngredients_ShouldMapTo_RecipeWithIngredients()
    {
        var dto = new CreateRecipeDto
        {
            UserId = 1,
            Name = "Pasta",
            Ingredients = new List<CreateIngredientDto>
            {
                new() { Name = "Spaghetti", Quantity = 400, Unit = "g", IsStaple = false },
                new() { Name = "Eggs", Quantity = 3, Unit = null, IsStaple = true }
            }
        };

        var recipe = _mapper.Map<Recipe>(dto);

        Assert.Equal(2, recipe.Ingredients.Count);
    }
}

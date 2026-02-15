using AutoMapper;
using RecipeVault.Application.DTOs;
using RecipeVault.Core.Entities;

namespace RecipeVault.Application.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<CreateRecipeDto, Recipe>();
        CreateMap<UpdateRecipeDto, Recipe>();
        CreateMap<Recipe, RecipeDto>();
    }
}

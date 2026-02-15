using RecipeVault.Core.Entities;
using RecipeVault.Core.Interfaces;
using RecipeVault.Infrastructure.Data;

namespace RecipeVault.Infrastructure.Repositories;

public class RecipeRepository : IRecipeRepository
{
    private readonly AppDbContext _context;

    public RecipeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Recipe?> GetByIdAsync(int id)
    {
        return await _context.Recipes.FindAsync(id);
    }

    public async Task<Recipe> AddAsync(Recipe recipe)
    {
        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();
        return recipe;
    }
}

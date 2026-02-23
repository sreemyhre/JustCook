using Microsoft.EntityFrameworkCore;
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
        return await _context.Recipes
            .Include(r => r.Ingredients)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Recipe>> GetAllByUserIdAsync(int userId)
    {
        return await _context.Recipes
            .Include(r => r.Ingredients)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Recipe> AddAsync(Recipe recipe)
    {
        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();
        return recipe;
    }

    public async Task UpdateAsync(Recipe recipe)
    {
        _context.Recipes.Update(recipe);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var recipe = await _context.Recipes.FindAsync(id);
        if (recipe == null) return false;

        _context.Recipes.Remove(recipe);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Recipe>> GetLeastRecentlyCookedAsync(int userId, int count)
    {
        return await _context.Recipes
            .Include(r => r.Ingredients)
            .Where(r => r.UserId == userId)
            .OrderBy(r => r.LastCookedDate)
            .Take(count)
            .ToListAsync();
    }
}

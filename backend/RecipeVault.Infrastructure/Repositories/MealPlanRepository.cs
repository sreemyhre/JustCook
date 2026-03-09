using Microsoft.EntityFrameworkCore;
using RecipeVault.Core.Entities;
using RecipeVault.Core.Interfaces;
using RecipeVault.Infrastructure.Data;

namespace RecipeVault.Infrastructure.Repositories;

public class MealPlanRepository : IMealPlanRepository
{
    private readonly AppDbContext _context;

    public MealPlanRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MealPlan?> GetByIdAsync(int id)
    {
        return await _context.MealPlans
            .Include(m => m.Items)
                .ThenInclude(i => i.Recipe)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<MealPlan>> GetAllByUserIdAsync(int userId)
    {
        return await _context.MealPlans
            .Include(m => m.Items)
                .ThenInclude(i => i.Recipe)
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.WeekStartDate)
            .ToListAsync();
    }

    public async Task<MealPlan> AddAsync(MealPlan mealPlan)
    {
        _context.MealPlans.Add(mealPlan);
        await _context.SaveChangesAsync();
        return mealPlan;
    }

    public async Task UpdateAsync(MealPlan mealPlan)
    {
        _context.MealPlans.Update(mealPlan);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var mealPlan = await _context.MealPlans.FindAsync(id);
        if (mealPlan == null) return false;

        _context.MealPlans.Remove(mealPlan);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Recipe>> GetUserRecipesWithTagsAsync(int userId)
    {
        return await _context.Recipes
            .Include(r => r.RecipeTags)
                .ThenInclude(rt => rt.Tag)
            .Where(r => r.UserId == userId)
            .ToListAsync();
    }
}

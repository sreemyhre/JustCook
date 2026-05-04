using Microsoft.EntityFrameworkCore;
using RecipeVault.Core.Entities;
using RecipeVault.Core.Interfaces;
using RecipeVault.Infrastructure.Data;

namespace RecipeVault.Infrastructure.Repositories;

public class TagRepository : ITagRepository
{
    private readonly AppDbContext _context;

    public TagRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Tag?> GetByIdAsync(int id, int userId)
    {
        return await _context.Tags.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    }

    public async Task<IEnumerable<Tag>> GetAllByUserIdAsync(int userId)
    {
        return await _context.Tags.Where(t => t.UserId == userId).OrderBy(t => t.Name).ToListAsync();
    }

    public async Task<IEnumerable<Recipe>> GetRecipesByTagIdAsync(int tagId, int userId)
    {
        return await _context.RecipeTags
            .Where(rt => rt.TagId == tagId && rt.Tag.UserId == userId)
            .Include(rt => rt.Recipe)
                .ThenInclude(r => r.Ingredients)
            .Select(rt => rt.Recipe)
            .ToListAsync();
    }

    public async Task<Tag> AddAsync(Tag tag)
    {
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();
        return tag;
    }

    public async Task UpdateAsync(Tag tag)
    {
        _context.Tags.Update(tag);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (tag == null) return false;

        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddTagToRecipeAsync(int recipeId, int tagId)
    {
        var exists = await _context.RecipeTags
            .AnyAsync(rt => rt.RecipeId == recipeId && rt.TagId == tagId);
        if (exists) return false;

        var recipeTag = new RecipeTag { RecipeId = recipeId, TagId = tagId };
        _context.RecipeTags.Add(recipeTag);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveTagFromRecipeAsync(int recipeId, int tagId)
    {
        var recipeTag = await _context.RecipeTags
            .FirstOrDefaultAsync(rt => rt.RecipeId == recipeId && rt.TagId == tagId);
        if (recipeTag == null) return false;

        _context.RecipeTags.Remove(recipeTag);
        await _context.SaveChangesAsync();
        return true;
    }
}

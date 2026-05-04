using AutoMapper;
using RecipeVault.Application.DTOs;
using RecipeVault.Application.Interfaces;
using RecipeVault.Core.Entities;
using RecipeVault.Core.Enums;
using RecipeVault.Core.Interfaces;

namespace RecipeVault.Application.Services;

public class MealPlanService : IMealPlanService
{
    private readonly IMealPlanRepository _mealPlanRepository;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IMapper _mapper;

    public MealPlanService(IMealPlanRepository mealPlanRepository, IRecipeRepository recipeRepository, IMapper mapper)
    {
        _mealPlanRepository = mealPlanRepository;
        _recipeRepository = recipeRepository;
        _mapper = mapper;
    }

    public async Task<MealPlanDto> CreateMealPlanAsync(CreateMealPlanDto dto)
    {
        var mealPlan = _mapper.Map<MealPlan>(dto);
        var created = await _mealPlanRepository.AddAsync(mealPlan);
        var result = await _mealPlanRepository.GetByIdAsync(created.Id);
        return _mapper.Map<MealPlanDto>(result!);
    }

    public async Task<MealPlanDto?> GetByIdAsync(int id)
    {
        var mealPlan = await _mealPlanRepository.GetByIdAsync(id);
        if (mealPlan == null) return null;
        return _mapper.Map<MealPlanDto>(mealPlan);
    }

    public async Task<IEnumerable<MealPlanDto>> GetAllByUserIdAsync(int userId)
    {
        var plans = await _mealPlanRepository.GetAllByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<MealPlanDto>>(plans);
    }

    public async Task<MealPlanDto?> UpdateMealPlanAsync(int id, CreateMealPlanDto dto)
    {
        var mealPlan = await _mealPlanRepository.GetByIdAsync(id);
        if (mealPlan == null) return null;

        var oldRecipeIds = mealPlan.Items.Select(i => i.RecipeId).ToHashSet();
        var newItemMap = dto.Items.ToDictionary(i => i.RecipeId, i => i.DayOfWeek);
        var newRecipeIds = newItemMap.Keys.ToHashSet();

        foreach (var recipeId in oldRecipeIds.Except(newRecipeIds))
        {
            var recipe = await _recipeRepository.GetByIdAsync(recipeId);
            if (recipe == null) continue;
            recipe.LastPlannedDate = null;
            await _recipeRepository.UpdateAsync(recipe);
        }

        foreach (var recipeId in newRecipeIds.Except(oldRecipeIds))
        {
            var recipe = await _recipeRepository.GetByIdAsync(recipeId);
            if (recipe == null) continue;
            recipe.LastPlannedDate = dto.WeekStartDate.AddDays((int)newItemMap[recipeId]);
            await _recipeRepository.UpdateAsync(recipe);
        }

        mealPlan.WeekStartDate = dto.WeekStartDate;
        mealPlan.Items.Clear();
        _mapper.Map(dto, mealPlan);
        await _mealPlanRepository.UpdateAsync(mealPlan);

        var result = await _mealPlanRepository.GetByIdAsync(id);
        return _mapper.Map<MealPlanDto>(result!);
    }

    public async Task<bool> DeleteMealPlanAsync(int id)
    {
        return await _mealPlanRepository.DeleteAsync(id);
    }

    public async Task<MealPlanDto> GenerateMealPlanAsync(GenerateMealPlanDto dto)
    {
        var allRecipes = (await _mealPlanRepository.GetUserRecipesWithTagsAsync(dto.UserId)).ToList();
        var days = Enum.GetValues<DayOfWeekEnum>();
        var selectedRecipes = new List<(Recipe recipe, DayOfWeekEnum day)>();
        var usedRecipeIds = new HashSet<int>();

        // Step 1: Fill tag-quota slots
        foreach (var quota in dto.TagQuotas)
        {
            var candidates = allRecipes
                .Where(r => r.RecipeTags.Any(rt => rt.TagId == quota.TagId) && !usedRecipeIds.Contains(r.Id))
                .OrderByDescending(r => CalculateScore(r))
                .Take(quota.Count)
                .ToList();

            foreach (var recipe in candidates)
                usedRecipeIds.Add(recipe.Id);

            selectedRecipes.AddRange(candidates.Select(r => (r, default(DayOfWeekEnum))));
        }

        // Step 2: Fill remaining slots (up to 7) with highest-scored unused recipes
        int remaining = 7 - selectedRecipes.Count;
        if (remaining > 0)
        {
            var autoFill = allRecipes
                .Where(r => !usedRecipeIds.Contains(r.Id))
                .OrderByDescending(r => CalculateScore(r))
                .Take(remaining)
                .ToList();

            selectedRecipes.AddRange(autoFill.Select(r => (r, default(DayOfWeekEnum))));
        }

        // Step 3: Assign days (Monday to Sunday)
        var capped = selectedRecipes.Take(7).ToList();
        var items = capped
            .Select((entry, index) => new MealPlanItem
            {
                RecipeId = entry.recipe.Id,
                DayOfWeek = days[index],
                MealType = MealType.Dinner
            })
            .ToList();

        // Step 4: Stamp LastCookedDate so the next generation scores these recipes lower
        for (int i = 0; i < capped.Count; i++)
        {
            capped[i].recipe.LastPlannedDate = dto.WeekStartDate.AddDays(i);
            await _recipeRepository.UpdateAsync(capped[i].recipe);
        }

        var mealPlan = new MealPlan
        {
            UserId = dto.UserId,
            WeekStartDate = dto.WeekStartDate,
            Items = items
        };

        var created = await _mealPlanRepository.AddAsync(mealPlan);
        var result = await _mealPlanRepository.GetByIdAsync(created.Id);
        return _mapper.Map<MealPlanDto>(result!);
    }

    public async Task<MealPlanDto> PreviewMealPlanAsync(GenerateMealPlanDto dto)
    {
        var allRecipes = (await _mealPlanRepository.GetUserRecipesWithTagsAsync(dto.UserId)).ToList();
        var days = Enum.GetValues<DayOfWeekEnum>();
        var selectedRecipes = new List<Recipe>();
        var usedRecipeIds = new HashSet<int>();

        foreach (var quota in dto.TagQuotas)
        {
            var candidates = allRecipes
                .Where(r => r.RecipeTags.Any(rt => rt.TagId == quota.TagId) && !usedRecipeIds.Contains(r.Id))
                .OrderByDescending(CalculateScore)
                .Take(quota.Count)
                .ToList();
            foreach (var recipe in candidates) usedRecipeIds.Add(recipe.Id);
            selectedRecipes.AddRange(candidates);
        }

        int remaining = 7 - selectedRecipes.Count;
        if (remaining > 0)
        {
            var autoFill = allRecipes
                .Where(r => !usedRecipeIds.Contains(r.Id))
                .OrderByDescending(CalculateScore)
                .Take(remaining)
                .ToList();
            selectedRecipes.AddRange(autoFill);
        }

        var items = selectedRecipes
            .Take(7)
            .Select((recipe, index) => new MealPlanItemDto
            {
                Id = 0,
                RecipeId = recipe.Id,
                RecipeName = recipe.Name,
                DayOfWeek = days[index],
                MealType = MealType.Dinner
            })
            .ToList();

        return new MealPlanDto
        {
            Id = 0,
            UserId = dto.UserId,
            WeekStartDate = dto.WeekStartDate,
            Items = items
        };
    }

    /// <summary>
    /// Rotation scoring algorithm from the README:
    /// - Recency score: max after 14 days since last cooked
    /// - Popularity score: slight boost for frequently cooked favourites
    /// - New recipes (CookCount = 0) get a mid-range score to surface them
    /// </summary>
    private static double CalculateScore(Recipe recipe)
    {
        var lastActivity = recipe.LastPlannedDate.HasValue && recipe.LastCookedDate.HasValue
            ? (recipe.LastPlannedDate > recipe.LastCookedDate ? recipe.LastPlannedDate : recipe.LastCookedDate)
            : (recipe.LastPlannedDate ?? recipe.LastCookedDate);

        var daysSince = (DateTime.UtcNow - (lastActivity ?? DateTime.MinValue)).Days;
        var recencyScore = Math.Min(daysSince / 14.0, 1.0);

        var popularityScore = recipe.CookCount > 0
            ? Math.Min(recipe.CookCount / 10.0, 0.5)
            : 0.25;

        return recencyScore + popularityScore;
    }
}

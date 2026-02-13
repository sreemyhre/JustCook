using RecipeVault.Core.Enums;

namespace RecipeVault.Core.Entities;

public class MealPlanItem
{
    public int Id { get; set; }
    public int MealPlanId { get; set; }
    public int RecipeId { get; set; }
    public DayOfWeekEnum DayOfWeek { get; set; }
    public MealType MealType { get; set; }

    public MealPlan MealPlan { get; set; } = null!;
    public Recipe Recipe { get; set; } = null!;
}

using RecipeVault.Core.Enums;

namespace RecipeVault.Application.DTOs;

public class CreateMealPlanItemDto
{
    public int RecipeId { get; set; }
    public DayOfWeekEnum DayOfWeek { get; set; }
    public MealType MealType { get; set; }
}

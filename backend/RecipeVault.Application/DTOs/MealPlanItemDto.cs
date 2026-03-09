using RecipeVault.Core.Enums;

namespace RecipeVault.Application.DTOs;

public class MealPlanItemDto
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    public string RecipeName { get; set; } = string.Empty;
    public DayOfWeekEnum DayOfWeek { get; set; }
    public MealType MealType { get; set; }
}

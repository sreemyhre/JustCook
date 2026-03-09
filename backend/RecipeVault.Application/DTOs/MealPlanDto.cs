namespace RecipeVault.Application.DTOs;

public class MealPlanDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime WeekStartDate { get; set; }
    public List<MealPlanItemDto> Items { get; set; } = new();
}

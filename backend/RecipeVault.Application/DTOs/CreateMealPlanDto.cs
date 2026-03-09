namespace RecipeVault.Application.DTOs;

public class CreateMealPlanDto
{
    public int UserId { get; set; }
    public DateTime WeekStartDate { get; set; }
    public List<CreateMealPlanItemDto> Items { get; set; } = new();
}

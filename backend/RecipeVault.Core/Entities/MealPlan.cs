namespace RecipeVault.Core.Entities;

public class MealPlan
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime WeekStartDate { get; set; }

    public User User { get; set; } = null!;
    public ICollection<MealPlanItem> Items { get; set; } = new List<MealPlanItem>();
}

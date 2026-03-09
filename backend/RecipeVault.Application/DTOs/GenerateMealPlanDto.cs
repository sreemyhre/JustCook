namespace RecipeVault.Application.DTOs;

public class GenerateMealPlanDto
{
    public int UserId { get; set; }
    public DateTime WeekStartDate { get; set; }

    /// <summary>
    /// Optional tag quotas. e.g. [{ tagId: 1, count: 3 }, { tagId: 2, count: 2 }]
    /// If empty or totals less than 7, remaining slots are auto-filled by rotation score.
    /// </summary>
    public List<TagQuotaDto> TagQuotas { get; set; } = new();
}

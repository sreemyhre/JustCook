namespace RecipeVault.Application.DTOs;

public class UpdateRecipeDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ServingSize { get; set; }
    public int PrepTimeMinutes { get; set; }
    public int CookTimeMinutes { get; set; }
    public string? ImageUrl { get; set; }
}

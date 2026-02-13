namespace RecipeVault.Core.Entities;

public class Ingredient
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
    public bool IsStaple { get; set; }

    public Recipe Recipe { get; set; } = null!;
}

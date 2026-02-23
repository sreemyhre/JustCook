namespace RecipeVault.Application.DTOs;

public class CreateIngredientDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
    public bool IsStaple { get; set; }
}

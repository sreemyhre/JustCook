namespace RecipeVault.Core.Entities;

public class PantryStaple
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;

    public User User { get; set; } = null!;
}

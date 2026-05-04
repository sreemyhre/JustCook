namespace RecipeVault.Application.DTOs;

public class CreateTagDto
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
}

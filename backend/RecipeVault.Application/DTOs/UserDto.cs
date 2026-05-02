namespace RecipeVault.Application.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PictureUrl { get; set; }
    public string Provider { get; set; } = string.Empty;
}

public class LoginRequestDto
{
    public string FirebaseToken { get; set; } = string.Empty;
    public string RecaptchaToken { get; set; } = string.Empty;
}

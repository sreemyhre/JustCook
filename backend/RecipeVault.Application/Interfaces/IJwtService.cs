using RecipeVault.Application.DTOs;

namespace RecipeVault.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(UserDto user);
    bool ValidateToken(string token, out int userId);
}

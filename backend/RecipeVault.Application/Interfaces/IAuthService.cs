using RecipeVault.Application.DTOs;

namespace RecipeVault.Application.Interfaces;

public interface IAuthService
{
    Task<UserDto> LoginAsync(string firebaseToken, string recaptchaToken);
    Task<UserDto?> GetUserByFirebaseUidAsync(string firebaseUid);
}

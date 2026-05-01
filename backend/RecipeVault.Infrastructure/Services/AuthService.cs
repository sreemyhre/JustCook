using System.Net.Http.Json;
using System.Text.Json.Serialization;
using FirebaseAdmin.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RecipeVault.Application.DTOs;
using RecipeVault.Application.Interfaces;
using RecipeVault.Core.Entities;
using RecipeVault.Infrastructure.Data;

namespace RecipeVault.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        AppDbContext db,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _db = db;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<UserDto> LoginAsync(string firebaseToken, string recaptchaToken)
    {
        await VerifyRecaptchaAsync(recaptchaToken);

        FirebaseToken decoded;
        try
        {
            decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(firebaseToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Firebase token verification failed: {Message}", ex.Message);
            throw new UnauthorizedAccessException("Invalid authentication token.");
        }

        var uid = decoded.Uid;
        var email = decoded.Claims.TryGetValue("email", out var emailClaim) ? emailClaim?.ToString() ?? string.Empty : string.Empty;
        var name = decoded.Claims.TryGetValue("name", out var nameClaim) ? nameClaim?.ToString() ?? string.Empty : string.Empty;
        var picture = decoded.Claims.TryGetValue("picture", out var picClaim) ? picClaim?.ToString() : null;
        var provider = ExtractProvider(decoded.Claims);

        var nameParts = name.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        var firstName = nameParts.Length > 0 ? nameParts[0] : null;
        var lastName = nameParts.Length > 1 ? nameParts[1] : null;

        var user = await _db.Users.FirstOrDefaultAsync(u => u.FirebaseUid == uid);
        if (user == null)
        {
            user = new User
            {
                FirebaseUid = uid,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                PictureUrl = picture,
                Provider = provider,
                CreatedAt = DateTime.UtcNow
            };
            _db.Users.Add(user);
        }
        else
        {
            user.Email = email;
            user.FirstName = firstName;
            user.LastName = lastName;
            user.PictureUrl = picture;
            user.Provider = provider;
        }

        await _db.SaveChangesAsync();
        return ToDto(user);
    }

    public async Task<UserDto?> GetUserByFirebaseUidAsync(string firebaseUid)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);
        return user == null ? null : ToDto(user);
    }

    private async Task VerifyRecaptchaAsync(string token)
    {
        var secretKey = _configuration["RecaptchaV3:SecretKey"]
            ?? throw new InvalidOperationException("RecaptchaV3:SecretKey is not configured");
        var minScore = double.TryParse(_configuration["RecaptchaV3:MinScore"], out var s) ? s : 0.5;

        var client = _httpClientFactory.CreateClient("recaptcha");
        var response = await client.PostAsync(
            $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={token}",
            null);

        var result = await response.Content.ReadFromJsonAsync<RecaptchaResponse>();
        if (result == null || !result.Success || result.Score < minScore)
        {
            _logger.LogWarning("reCAPTCHA failed. Success={Success}, Score={Score}", result?.Success, result?.Score);
            throw new UnauthorizedAccessException("Bot protection check failed. Please try again.");
        }
    }

    private static string ExtractProvider(IReadOnlyDictionary<string, object> claims)
    {
        if (!claims.TryGetValue("firebase", out var fbObj)) return "unknown";
        var fb = fbObj?.ToString() ?? string.Empty;
        if (fb.Contains("google.com")) return "google";
        if (fb.Contains("apple.com")) return "apple";
        if (fb.Contains("facebook.com")) return "facebook";
        return "unknown";
    }

    private static UserDto ToDto(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        PictureUrl = user.PictureUrl,
        Provider = user.Provider
    };

    private class RecaptchaResponse
    {
        [JsonPropertyName("success")] public bool Success { get; set; }
        [JsonPropertyName("score")] public double Score { get; set; }
        [JsonPropertyName("action")] public string Action { get; set; } = string.Empty;
    }
}

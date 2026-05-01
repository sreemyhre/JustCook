using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RecipeVault.Application.DTOs;
using RecipeVault.Application.Interfaces;

namespace RecipeVault.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly string _key;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expiryDays;

    public JwtService(IConfiguration configuration)
    {
        _key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured");
        _issuer = configuration["Jwt:Issuer"] ?? "justcook-api";
        _audience = configuration["Jwt:Audience"] ?? "justcook-app";
        _expiryDays = int.TryParse(configuration["Jwt:ExpiryDays"], out var days) ? days : 7;
    }

    public string GenerateToken(UserDto user)
    {
        var keyBytes = Encoding.UTF8.GetBytes(_key);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(keyBytes),
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("first_name", user.FirstName ?? string.Empty),
            new Claim("last_name", user.LastName ?? string.Empty),
            new Claim("picture", user.PictureUrl ?? string.Empty),
            new Claim("provider", user.Provider),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(_expiryDays),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool ValidateToken(string token, out int userId)
    {
        userId = 0;
        try
        {
            var keyBytes = Encoding.UTF8.GetBytes(_key);
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out _);

            var sub = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return int.TryParse(sub, out userId);
        }
        catch
        {
            return false;
        }
    }
}

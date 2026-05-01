using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeVault.Application.DTOs;
using RecipeVault.Application.Interfaces;

namespace RecipeVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IJwtService _jwtService;

    public AuthController(IAuthService authService, IJwtService jwtService)
    {
        _authService = authService;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var user = await _authService.LoginAsync(request.FirebaseToken, request.RecaptchaToken);
            var token = _jwtService.GenerateToken(user);

            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                Path = "/"
            });

            return Ok(user);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/"
        });
        return Ok();
    }

    [HttpGet("me")]
    [Authorize]
    public ActionResult<UserDto> Me()
    {
        var sub = User.FindFirst("sub")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (sub == null || !int.TryParse(sub, out var userId))
            return Unauthorized();

        return Ok(new UserDto
        {
            Id = userId,
            Email = User.FindFirst("email")?.Value ?? string.Empty,
            FirstName = User.FindFirst("first_name")?.Value,
            LastName = User.FindFirst("last_name")?.Value,
            PictureUrl = User.FindFirst("picture")?.Value,
            Provider = User.FindFirst("provider")?.Value ?? string.Empty
        });
    }
}

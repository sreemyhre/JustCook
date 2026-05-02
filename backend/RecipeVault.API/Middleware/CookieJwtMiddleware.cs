namespace RecipeVault.API.Middleware;

public class CookieJwtMiddleware
{
    private readonly RequestDelegate _next;

    public CookieJwtMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        // If no Authorization header but a jwt cookie exists, promote it
        if (!context.Request.Headers.ContainsKey("Authorization") &&
            context.Request.Cookies.TryGetValue("jwt", out var token) &&
            !string.IsNullOrEmpty(token))
        {
            context.Request.Headers.Append("Authorization", $"Bearer {token}");
        }

        await _next(context);
    }
}

using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Primitives;

public static class TokenExtensions
{
    public static string? GetSubFromHttpContext(this HttpContext httpContext)
    {
        if (!httpContext.Request.Headers.TryGetValue("Authorization", out StringValues values))
        {
            string? bearerToken = values.FirstOrDefault();
            return GetSubFromBearerToken(bearerToken);
        }
        return null;
    }
    public static string? GetSubFromBearerToken(this string? tokenString)
    {
        if (string.IsNullOrEmpty(tokenString) || !tokenString.Contains("Bearer"))
        {
            return null;
        }
        tokenString = tokenString.Split("Bearer ")[1];
        JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken token = jwtSecurityTokenHandler.ReadJwtToken(tokenString);

        return token.Payload.TryGetValue("sub", out object? value) ? (string)value : null;
    }
}
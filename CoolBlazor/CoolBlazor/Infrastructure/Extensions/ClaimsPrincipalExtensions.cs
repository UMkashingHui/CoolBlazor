using System.Security.Claims;

namespace CoolBlazor.Infrastructure.Extensions
{
    internal static class ClaimsPrincipalExtensions
    {
        internal static string GetEmail(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.FindFirst(ClaimTypes.Email).Value;

        internal static string GetFirstName(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.FindFirst(ClaimTypes.Name).Value;

        internal static string GetLastName(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.FindFirst(ClaimTypes.Surname).Value;

        internal static string GetPhoneNumber(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.FindFirst(ClaimTypes.MobilePhone).Value;

        internal static string GetUserId(this ClaimsPrincipal claimsPrincipal)
           => claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier).Value;
    }
}
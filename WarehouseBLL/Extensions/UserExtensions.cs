using System.Security.Claims;

namespace WarehouseBLL.Extensions;

public static class UserExtensions
{
    public static int? GetUserId(this ClaimsPrincipal Claims)
    {
        var userId = int.Parse(Claims.FindFirstValue(ClaimTypes.NameIdentifier));
        return userId;
    }
    public static string? GetGivenName(this ClaimsPrincipal Claims)
    {
        var givenName = Claims.FindFirstValue(ClaimTypes.GivenName);
        return givenName;
    }
}

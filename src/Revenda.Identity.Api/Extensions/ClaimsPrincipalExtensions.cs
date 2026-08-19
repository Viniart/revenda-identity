using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Revenda.Identity.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetCustomerId(this ClaimsPrincipal principal)
    {
        var subject = principal.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(subject, out var customerId)
            ? customerId
            : throw new UnauthorizedAccessException("Token sem identificador de cliente.");
    }
}

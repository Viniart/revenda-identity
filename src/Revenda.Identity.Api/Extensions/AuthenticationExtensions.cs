using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Revenda.Identity.Application.Ports.Output;
using Revenda.Identity.Infrastructure.Security;

namespace Revenda.Identity.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddIdentityAuthentication(this IServiceCollection services)
    {
        // Sem o mapeamento legado, "sub" e "role" chegam ao ClaimsPrincipal com o nome original.
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

        services
            .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IJsonWebKeySetProvider, IOptions<JwtOptions>>((bearer, keyProvider, jwtOptions) =>
            {
                var jwt = jwtOptions.Value;

                bearer.MapInboundClaims = false;
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    NameClaimType = TokenClaims.Subject,
                    RoleClaimType = TokenClaims.Role,
                    IssuerSigningKeyResolver = (_, _, keyId, _) =>
                        ToSecurityKeys(keyProvider.GetPublicKeys(), keyId)
                };
            });

        services.AddAuthorization();

        return services;
    }

    private static IEnumerable<SecurityKey> ToSecurityKeys(JsonWebKeySetDocument document, string? keyId) =>
        document.Keys
            .Where(key => keyId is null || key.Kid == keyId)
            .Select(key => new RsaSecurityKey(new RSAParameters
            {
                Modulus = Base64UrlEncoder.DecodeBytes(key.N),
                Exponent = Base64UrlEncoder.DecodeBytes(key.E)
            })
            {
                KeyId = key.Kid
            });
}

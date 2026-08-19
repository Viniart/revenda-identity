using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Revenda.Identity.Application.Ports.Output;
using Revenda.Identity.Domain.Entities;

namespace Revenda.Identity.Infrastructure.Security;

internal sealed class JwtAccessTokenIssuer : IAccessTokenIssuer
{
    private readonly JwtOptions _options;
    private readonly SigningKeyProvider _signingKeys;
    private readonly IClock _clock;

    public JwtAccessTokenIssuer(IOptions<JwtOptions> options, SigningKeyProvider signingKeys, IClock clock)
    {
        _options = options.Value;
        _signingKeys = signingKeys;
        _clock = clock;
    }

    public AccessToken Issue(Customer customer)
    {
        var issuedAt = _clock.UtcNow;
        var expiresAt = issuedAt.AddMinutes(_options.AccessTokenLifetimeMinutes);

        // Apenas o identificador e o papel: nome, CPF e e-mail não trafegam para o serviço de vendas.
        Claim[] claims =
        [
            new(JwtRegisteredClaimNames.Sub, customer.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(TokenClaims.Role, customer.Role.ToString())
        ];

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: issuedAt.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: _signingKeys.SigningCredentials);

        return new AccessToken(new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}

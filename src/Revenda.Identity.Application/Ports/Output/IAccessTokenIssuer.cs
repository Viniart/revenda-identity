using Revenda.Identity.Domain.Entities;

namespace Revenda.Identity.Application.Ports.Output;

public interface IAccessTokenIssuer
{
    AccessToken Issue(Customer customer);
}

public sealed record AccessToken(string Value, DateTimeOffset ExpiresAt);

/// <summary>
/// Nomes de claim que compõem o contrato do token entre este serviço e quem o valida.
/// </summary>
public static class TokenClaims
{
    public const string Subject = "sub";

    public const string Role = "role";
}

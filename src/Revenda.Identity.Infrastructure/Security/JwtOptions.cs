namespace Revenda.Identity.Infrastructure.Security;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "https://localhost:8081";

    public string Audience { get; set; } = "revenda-vehicles";

    public int AccessTokenLifetimeMinutes { get; set; } = 60;

    public string KeyId { get; set; } = "revenda-identity";

    /// <summary>
    /// Chave privada RSA em PEM. Quando ausente, uma chave efêmera é gerada na subida,
    /// o que só é aceitável em desenvolvimento e nos testes.
    /// </summary>
    public string? PrivateKeyPem { get; set; }
}

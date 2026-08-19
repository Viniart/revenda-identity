using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Revenda.Identity.Application.Ports.Output;

namespace Revenda.Identity.Infrastructure.Security;

internal sealed class SigningKeyProvider : IJsonWebKeySetProvider, IDisposable
{
    private readonly RSA _rsa;
    private readonly RsaSecurityKey _securityKey;

    public SigningKeyProvider(IOptions<JwtOptions> options, ILogger<SigningKeyProvider> logger)
    {
        var settings = options.Value;
        _rsa = RSA.Create(2048);

        if (string.IsNullOrWhiteSpace(settings.PrivateKeyPem))
        {
            logger.LogWarning(
                "Jwt:PrivateKeyPem não configurada. Uma chave efêmera foi gerada e os tokens " +
                "emitidos deixam de ser válidos a cada reinício do serviço.");
        }
        else
        {
            _rsa.ImportFromPem(settings.PrivateKeyPem);
        }

        _securityKey = new RsaSecurityKey(_rsa) { KeyId = settings.KeyId };
    }

    public SigningCredentials SigningCredentials => new(_securityKey, SecurityAlgorithms.RsaSha256);

    public JsonWebKeySetDocument GetPublicKeys()
    {
        var parameters = _rsa.ExportParameters(includePrivateParameters: false);

        var key = new JsonWebKeyDocument(
            Kty: "RSA",
            Use: "sig",
            Kid: _securityKey.KeyId,
            Alg: SecurityAlgorithms.RsaSha256,
            N: Base64UrlEncoder.Encode(parameters.Modulus),
            E: Base64UrlEncoder.Encode(parameters.Exponent));

        return new JsonWebKeySetDocument([key]);
    }

    public void Dispose() => _rsa.Dispose();
}

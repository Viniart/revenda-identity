namespace Revenda.Identity.Application.Ports.Output;

/// <summary>
/// Publica a parte pública da chave de assinatura para que outros serviços validem
/// o token sem compartilhar segredo.
/// </summary>
public interface IJsonWebKeySetProvider
{
    JsonWebKeySetDocument GetPublicKeys();
}

public sealed record JsonWebKeySetDocument(IReadOnlyList<JsonWebKeyDocument> Keys);

public sealed record JsonWebKeyDocument(string Kty, string Use, string Kid, string Alg, string N, string E);

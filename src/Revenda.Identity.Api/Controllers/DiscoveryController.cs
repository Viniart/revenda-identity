using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Revenda.Identity.Application.Ports.Output;
using Revenda.Identity.Infrastructure.Security;

namespace Revenda.Identity.Api.Controllers;

/// <summary>
/// Metadados públicos consumidos pelo serviço de veículos para validar o token sem
/// nenhum segredo compartilhado entre os dois serviços.
/// </summary>
[ApiController]
[Route(".well-known")]
[AllowAnonymous]
[Produces("application/json")]
public sealed class DiscoveryController : ControllerBase
{
    private readonly IJsonWebKeySetProvider _keys;
    private readonly JwtOptions _jwt;

    public DiscoveryController(IJsonWebKeySetProvider keys, IOptions<JwtOptions> jwt)
    {
        _keys = keys;
        _jwt = jwt.Value;
    }

    [HttpGet("jwks.json")]
    [ProducesResponseType(typeof(JsonWebKeySetDocument), StatusCodes.Status200OK)]
    public ActionResult<JsonWebKeySetDocument> Jwks() => _keys.GetPublicKeys();

    [HttpGet("openid-configuration")]
    public IActionResult OpenIdConfiguration()
    {
        var issuer = _jwt.Issuer.TrimEnd('/');

        return Ok(new Dictionary<string, object>
        {
            ["issuer"] = issuer,
            ["jwks_uri"] = $"{issuer}/.well-known/jwks.json",
            ["token_endpoint"] = $"{issuer}/auth/login",
            ["response_types_supported"] = new[] { "token" },
            ["subject_types_supported"] = new[] { "public" },
            ["id_token_signing_alg_values_supported"] = new[] { "RS256" }
        });
    }
}

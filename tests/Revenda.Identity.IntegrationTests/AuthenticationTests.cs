using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace Revenda.Identity.IntegrationTests;

public class AuthenticationTests : IClassFixture<IdentityApiFactory>
{
    private readonly HttpClient _client;

    public AuthenticationTests(IdentityApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Login_DeveAutenticarOAdministradorCriadoNaSubida()
    {
        var resposta = await _client.PostAsJsonAsync("/auth/login", new
        {
            email = IdentityApiFactory.AdministratorEmail,
            password = IdentityApiFactory.AdministratorPassword
        });

        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Login_DeveResponder401_QuandoSenhaEstaErrada()
    {
        var resposta = await _client.PostAsJsonAsync("/auth/login", new
        {
            email = IdentityApiFactory.AdministratorEmail,
            password = "SenhaErrada1"
        });

        resposta.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_DeveResponder401_QuandoEmailNaoExiste()
    {
        var resposta = await _client.PostAsJsonAsync("/auth/login", new
        {
            email = "ninguem@revenda.com",
            password = "Revenda2026"
        });

        resposta.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Jwks_DevePublicarAChavePublicaDeAssinatura()
    {
        var jwks = await _client.GetFromJsonAsync<JwksResponse>("/.well-known/jwks.json");

        jwks!.Keys.Should().ContainSingle();
        jwks.Keys[0].Kty.Should().Be("RSA");
        jwks.Keys[0].Alg.Should().Be("RS256");
        jwks.Keys[0].N.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Discovery_DeveApontarParaOEndpointDeChaves()
    {
        var documento = await _client.GetFromJsonAsync<Dictionary<string, object>>(
            "/.well-known/openid-configuration");

        documento!["jwks_uri"].ToString().Should().EndWith("/.well-known/jwks.json");
    }

    private sealed record JwksResponse(IReadOnlyList<JwkResponse> Keys);

    private sealed record JwkResponse(string Kty, string Use, string Kid, string Alg, string N, string E);
}

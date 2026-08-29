using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;

namespace Revenda.Identity.IntegrationTests;

public class CustomerRegistrationTests : IClassFixture<IdentityApiFactory>
{
    private readonly IdentityApiFactory _factory;
    private readonly HttpClient _client;

    public CustomerRegistrationTests(IdentityApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Cadastro_LoginEConsultaDePerfil_DevemFuncionarEmSequencia()
    {
        var email = $"{Guid.NewGuid():N}@revenda.com";

        var cadastro = await _client.PostAsJsonAsync("/customers", new
        {
            name = "Ana Silva",
            cpf = GerarCpf(),
            email,
            password = "Revenda2026"
        });

        cadastro.StatusCode.Should().Be(HttpStatusCode.Created);

        var token = await AutenticarAsync(email, "Revenda2026");
        token.Should().NotBeNullOrWhiteSpace();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var perfil = await _client.GetFromJsonAsync<PerfilResponse>("/customers/me");

        perfil!.Email.Should().Be(email);
        perfil.Role.Should().Be("Buyer");
    }

    [Fact]
    public async Task Cadastro_DeveResponder409_QuandoEmailJaExiste()
    {
        var email = $"{Guid.NewGuid():N}@revenda.com";
        var payload = new { name = "Ana Silva", cpf = GerarCpf(), email, password = "Revenda2026" };

        (await _client.PostAsJsonAsync("/customers", payload)).EnsureSuccessStatusCode();

        var repetido = await _client.PostAsJsonAsync("/customers", new
        {
            name = "Outra Pessoa",
            cpf = GerarCpf(),
            email,
            password = "Revenda2026"
        });

        repetido.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Cadastro_DeveResponder400_QuandoCpfEInvalido()
    {
        var resposta = await _client.PostAsJsonAsync("/customers", new
        {
            name = "Ana Silva",
            cpf = "11111111111",
            email = $"{Guid.NewGuid():N}@revenda.com",
            password = "Revenda2026"
        });

        resposta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Perfil_DeveResponder401_SemToken()
    {
        // O cliente da fábrica fala com o servidor em memória. Um HttpClient comum tentaria
        // uma conexão TCP real, que não existe, e o teste falharia antes de chegar à API.
        var semToken = _factory.CreateClient();

        var resposta = await semToken.GetAsync("/customers/me");

        resposta.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task<string?> AutenticarAsync(string email, string password)
    {
        var resposta = await _client.PostAsJsonAsync("/auth/login", new { email, password });
        resposta.EnsureSuccessStatusCode();

        var conteudo = await resposta.Content.ReadFromJsonAsync<LoginResponse>();
        return conteudo?.AccessToken;
    }

    /// <summary>CPF sintético com dígitos verificadores válidos, para não colidir entre execuções.</summary>
    private static string GerarCpf()
    {
        var random = Random.Shared;
        var digits = new int[11];

        for (var i = 0; i < 9; i++)
        {
            digits[i] = random.Next(0, 10);
        }

        digits[9] = CalcularDigito(digits, 9);
        digits[10] = CalcularDigito(digits, 10);

        return string.Concat(digits);
    }

    private static int CalcularDigito(IReadOnlyList<int> digits, int take)
    {
        var weight = take + 1;
        var sum = 0;

        for (var i = 0; i < take; i++)
        {
            sum += digits[i] * weight--;
        }

        var remainder = sum % 11;
        return remainder < 2 ? 0 : 11 - remainder;
    }

    private sealed record LoginResponse(string AccessToken, string TokenType, long ExpiresIn);

    private sealed record PerfilResponse(Guid Id, string Name, string Cpf, string Email, string Role);
}

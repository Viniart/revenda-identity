using FluentAssertions;
using NSubstitute;
using Revenda.Identity.Application.Exceptions;
using Revenda.Identity.Application.Ports.Output;
using Revenda.Identity.Application.UseCases.Authentication;
using Revenda.Identity.Domain.Entities;
using Revenda.Identity.Domain.ValueObjects;
using Revenda.Identity.UnitTests.TestData;

namespace Revenda.Identity.UnitTests.Application;

public class AuthenticateCustomerUseCaseTests
{
    private readonly ICustomerRepository _customers = Substitute.For<ICustomerRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IAccessTokenIssuer _tokenIssuer = Substitute.For<IAccessTokenIssuer>();
    private readonly AuthenticateCustomerUseCase _useCase;

    public AuthenticateCustomerUseCaseTests()
    {
        _useCase = new AuthenticateCustomerUseCase(
            _customers,
            _passwordHasher,
            _tokenIssuer,
            new FixedClock(CustomerFactory.Agora));
    }

    [Fact]
    public async Task ExecuteAsync_DeveEmitirTokenComValidadeEmSegundos_QuandoCredenciaisConferem()
    {
        var customer = CustomerFactory.Comprador();
        _customers.FindByEmailAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>()).Returns(customer);
        _passwordHasher.Verify("hash-armazenado", "Revenda2026").Returns(true);
        _tokenIssuer.Issue(customer).Returns(new AccessToken("jwt", CustomerFactory.Agora.AddHours(1)));

        var output = await _useCase.ExecuteAsync(
            new AuthenticateCustomerInput("ana@revenda.com", "Revenda2026"),
            CancellationToken.None);

        output.AccessToken.Should().Be("jwt");
        output.TokenType.Should().Be("Bearer");
        output.ExpiresIn.Should().Be(3600);
    }

    [Fact]
    public async Task ExecuteAsync_DeveLancarExcecao_QuandoSenhaNaoConfere()
    {
        _customers
            .FindByEmailAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns(CustomerFactory.Comprador());
        _passwordHasher.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        var executar = () => _useCase.ExecuteAsync(
            new AuthenticateCustomerInput("ana@revenda.com", "errada123"),
            CancellationToken.None);

        await executar.Should().ThrowAsync<InvalidCredentialsException>();
        _tokenIssuer.DidNotReceiveWithAnyArgs().Issue(default!);
    }

    [Fact]
    public async Task ExecuteAsync_DeveVerificarHashMesmoSemCadastro_ParaNaoRevelarEmailsExistentes()
    {
        _customers
            .FindByEmailAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns((Customer?)null);

        var executar = () => _useCase.ExecuteAsync(
            new AuthenticateCustomerInput("desconhecido@revenda.com", "Revenda2026"),
            CancellationToken.None);

        await executar.Should().ThrowAsync<InvalidCredentialsException>();
        _passwordHasher.ReceivedWithAnyArgs(1).Verify(default!, default!);
    }

    [Theory]
    [InlineData("sem-arroba")]
    [InlineData("")]
    [InlineData(null)]
    public async Task ExecuteAsync_DeveTratarEmailMalformadoComoCredencialInvalida(string? email)
    {
        var executar = () => _useCase.ExecuteAsync(
            new AuthenticateCustomerInput(email, "Revenda2026"),
            CancellationToken.None);

        await executar.Should().ThrowAsync<InvalidCredentialsException>();
        await _customers.DidNotReceiveWithAnyArgs().FindByEmailAsync(default!, default);
    }
}

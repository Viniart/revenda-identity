using FluentAssertions;
using NSubstitute;
using Revenda.Identity.Application.Exceptions;
using Revenda.Identity.Application.Ports.Output;
using Revenda.Identity.Application.UseCases.Customers;
using Revenda.Identity.Domain.Entities;
using Revenda.Identity.Domain.Enums;
using Revenda.Identity.Domain.Exceptions;
using Revenda.Identity.Domain.ValueObjects;
using Revenda.Identity.UnitTests.TestData;

namespace Revenda.Identity.UnitTests.Application;

public class RegisterCustomerUseCaseTests
{
    private readonly ICustomerRepository _customers = Substitute.For<ICustomerRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly RegisterCustomerUseCase _useCase;

    public RegisterCustomerUseCaseTests()
    {
        _passwordHasher.Hash(Arg.Any<Password>()).Returns("hash-gerado");
        _useCase = new RegisterCustomerUseCase(
            _customers,
            _passwordHasher,
            _unitOfWork,
            new FixedClock(CustomerFactory.Agora));
    }

    [Fact]
    public async Task ExecuteAsync_DevePersistirCompradorEDevolverPerfil()
    {
        Customer? persistido = null;
        await _customers.AddAsync(
            Arg.Do<Customer>(c => persistido = c),
            Arg.Any<CancellationToken>());

        var output = await _useCase.ExecuteAsync(EntradaValida(), CancellationToken.None);

        persistido.Should().NotBeNull();
        persistido!.PasswordHash.Should().Be("hash-gerado");
        persistido.Role.Should().Be(CustomerRole.Buyer);

        output.Email.Should().Be("ana@revenda.com");
        output.Cpf.Should().Be("529.982.247-25");
        output.Role.Should().Be(nameof(CustomerRole.Buyer));

        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_DeveLancarExcecao_QuandoEmailJaEstaCadastrado()
    {
        _customers
            .ExistsByEmailAsync(Arg.Any<Email>(), null, Arg.Any<CancellationToken>())
            .Returns(true);

        var executar = () => _useCase.ExecuteAsync(EntradaValida(), CancellationToken.None);

        await executar.Should().ThrowAsync<DuplicateCustomerException>();
        await _customers.DidNotReceive().AddAsync(Arg.Any<Customer>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_DeveLancarExcecao_QuandoCpfJaEstaCadastrado()
    {
        _customers.ExistsByCpfAsync(Arg.Any<Cpf>(), Arg.Any<CancellationToken>()).Returns(true);

        var executar = () => _useCase.ExecuteAsync(EntradaValida(), CancellationToken.None);

        await executar.Should().ThrowAsync<DuplicateCustomerException>();
    }

    [Fact]
    public async Task ExecuteAsync_NaoDeveConsultarRepositorio_QuandoDadosSaoInvalidos()
    {
        var entrada = EntradaValida() with { Cpf = "000" };

        var executar = () => _useCase.ExecuteAsync(entrada, CancellationToken.None);

        await executar.Should().ThrowAsync<InvalidCustomerDataException>();
        await _customers.DidNotReceiveWithAnyArgs().ExistsByEmailAsync(default!, default, default);
    }

    [Fact]
    public async Task ExecuteAsync_NaoDeveGravar_QuandoSenhaNaoAtendeAPolitica()
    {
        var entrada = EntradaValida() with { Password = "curta" };

        var executar = () => _useCase.ExecuteAsync(entrada, CancellationToken.None);

        await executar.Should().ThrowAsync<InvalidCustomerDataException>();
        await _unitOfWork.DidNotReceiveWithAnyArgs().CommitAsync(default);
    }

    private static RegisterCustomerInput EntradaValida() =>
        new("Ana Silva", "529.982.247-25", "Ana@Revenda.com", "Revenda2026");
}

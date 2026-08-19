using FluentAssertions;
using NSubstitute;
using Revenda.Identity.Application.Exceptions;
using Revenda.Identity.Application.Ports.Output;
using Revenda.Identity.Application.UseCases.Customers;
using Revenda.Identity.Domain.Entities;
using Revenda.Identity.Domain.ValueObjects;
using Revenda.Identity.UnitTests.TestData;

namespace Revenda.Identity.UnitTests.Application;

public class UpdateCustomerProfileUseCaseTests
{
    private readonly ICustomerRepository _customers = Substitute.For<ICustomerRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly UpdateCustomerProfileUseCase _useCase;

    public UpdateCustomerProfileUseCaseTests() =>
        _useCase = new UpdateCustomerProfileUseCase(
            _customers,
            _unitOfWork,
            new FixedClock(CustomerFactory.Agora.AddDays(2)));

    [Fact]
    public async Task ExecuteAsync_DeveAtualizarPerfilERegistrarAAlteracao()
    {
        var customer = CustomerFactory.Comprador();
        _customers.FindByIdAsync(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);

        var output = await _useCase.ExecuteAsync(
            new UpdateCustomerProfileInput(customer.Id, "Ana Souza", "ana.souza@revenda.com"),
            CancellationToken.None);

        output.Name.Should().Be("Ana Souza");
        customer.UpdatedAt.Should().Be(CustomerFactory.Agora.AddDays(2));
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_DeveLancarExcecao_QuandoClienteNaoExiste()
    {
        _customers.FindByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Customer?)null);

        var executar = () => _useCase.ExecuteAsync(
            new UpdateCustomerProfileInput(Guid.NewGuid(), "Ana", "ana@revenda.com"),
            CancellationToken.None);

        await executar.Should().ThrowAsync<CustomerNotFoundException>();
    }

    [Fact]
    public async Task ExecuteAsync_DeveIgnorarOProprioCadastroAoChecarDuplicidadeDeEmail()
    {
        var customer = CustomerFactory.Comprador();
        _customers.FindByIdAsync(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);

        await _useCase.ExecuteAsync(
            new UpdateCustomerProfileInput(customer.Id, "Ana Silva", "ana@revenda.com"),
            CancellationToken.None);

        await _customers.Received(1).ExistsByEmailAsync(
            Arg.Any<Email>(),
            customer.Id,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_DeveLancarExcecao_QuandoEmailPertenceAOutroCliente()
    {
        var customer = CustomerFactory.Comprador();
        _customers.FindByIdAsync(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);
        _customers
            .ExistsByEmailAsync(Arg.Any<Email>(), customer.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        var executar = () => _useCase.ExecuteAsync(
            new UpdateCustomerProfileInput(customer.Id, "Ana", "usada@revenda.com"),
            CancellationToken.None);

        await executar.Should().ThrowAsync<DuplicateCustomerException>();
        await _unitOfWork.DidNotReceiveWithAnyArgs().CommitAsync(default);
    }
}

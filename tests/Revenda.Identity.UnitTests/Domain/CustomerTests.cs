using FluentAssertions;
using Revenda.Identity.Domain.Entities;
using Revenda.Identity.Domain.Enums;
using Revenda.Identity.Domain.Exceptions;
using Revenda.Identity.Domain.ValueObjects;

namespace Revenda.Identity.UnitTests.Domain;

public class CustomerTests
{
    private static readonly DateTimeOffset Agora = new(2026, 3, 10, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Register_DeveCriarCompradorComDadosNormalizados()
    {
        var customer = Criar(name: "  Ana Silva  ");

        customer.Id.Should().NotBeEmpty();
        customer.Name.Should().Be("Ana Silva");
        customer.Role.Should().Be(CustomerRole.Buyer);
        customer.CreatedAt.Should().Be(Agora);
        customer.UpdatedAt.Should().Be(Agora);
    }

    [Fact]
    public void RegisterAdministrator_DeveAtribuirPapelAdministrativo()
    {
        var admin = Customer.RegisterAdministrator(
            "Operador",
            Cpf.Create("52998224725"),
            Email.Create("operador@revenda.com"),
            "hash",
            Agora);

        admin.Role.Should().Be(CustomerRole.Administrator);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Register_DeveLancarExcecao_QuandoNomeEInvalido(string? name)
    {
        var criar = () => Criar(name);

        criar.Should().Throw<InvalidCustomerDataException>();
    }

    [Fact]
    public void Register_DeveLancarExcecao_QuandoHashEstaVazio()
    {
        var criar = () => Customer.Register(
            "Ana Silva",
            Cpf.Create("52998224725"),
            Email.Create("ana@revenda.com"),
            "  ",
            Agora);

        criar.Should().Throw<InvalidCustomerDataException>();
    }

    [Fact]
    public void ChangeProfile_DeveAtualizarNomeEmailEDataDeAlteracao()
    {
        var customer = Criar();
        var depois = Agora.AddDays(1);

        customer.ChangeProfile("Ana Souza", Email.Create("ana.souza@revenda.com"), depois);

        customer.Name.Should().Be("Ana Souza");
        customer.Email.Value.Should().Be("ana.souza@revenda.com");
        customer.UpdatedAt.Should().Be(depois);
        customer.CreatedAt.Should().Be(Agora);
    }

    [Fact]
    public void ChangePassword_DeveSubstituirOHash()
    {
        var customer = Criar();

        customer.ChangePassword("novo-hash", Agora.AddHours(2));

        customer.PasswordHash.Should().Be("novo-hash");
        customer.UpdatedAt.Should().Be(Agora.AddHours(2));
    }

    private static Customer Criar(string? name = "Ana Silva") =>
        Customer.Register(
            name,
            Cpf.Create("52998224725"),
            Email.Create("ana@revenda.com"),
            "hash",
            Agora);
}

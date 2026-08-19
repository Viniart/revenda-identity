using FluentAssertions;
using Revenda.Identity.Domain.Exceptions;
using Revenda.Identity.Domain.ValueObjects;

namespace Revenda.Identity.UnitTests.Domain;

public class EmailTests
{
    [Fact]
    public void Create_DeveNormalizarCaixaEEspacos()
    {
        Email.Create("  Ana.Silva@Revenda.COM  ").Value.Should().Be("ana.silva@revenda.com");
    }

    [Theory]
    [InlineData("ana")]
    [InlineData("ana@")]
    [InlineData("@revenda.com")]
    [InlineData("Ana Silva <ana@revenda.com>")]
    [InlineData("")]
    [InlineData(null)]
    public void Create_DeveLancarExcecao_QuandoFormatoEInvalido(string? input)
    {
        var criar = () => Email.Create(input);

        criar.Should().Throw<InvalidCustomerDataException>();
    }

    [Fact]
    public void Create_DeveLancarExcecao_QuandoExcedeOTamanhoMaximo()
    {
        var longo = new string('a', Email.MaxLength) + "@revenda.com";

        var criar = () => Email.Create(longo);

        criar.Should().Throw<InvalidCustomerDataException>();
    }
}

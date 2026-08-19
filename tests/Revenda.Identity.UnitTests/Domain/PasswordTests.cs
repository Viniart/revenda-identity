using FluentAssertions;
using Revenda.Identity.Domain.Exceptions;
using Revenda.Identity.Domain.ValueObjects;

namespace Revenda.Identity.UnitTests.Domain;

public class PasswordTests
{
    [Fact]
    public void Create_DevePreservarOValorOriginal()
    {
        Password.Create("Revenda2026").Value.Should().Be("Revenda2026");
    }

    [Theory]
    [InlineData("abc1")]
    [InlineData("somenteletras")]
    [InlineData("12345678")]
    [InlineData(null)]
    public void Create_DeveLancarExcecao_QuandoNaoAtendeAPolitica(string? input)
    {
        var criar = () => Password.Create(input);

        criar.Should().Throw<InvalidCustomerDataException>();
    }

    [Fact]
    public void ToString_NaoDeveExporOValor()
    {
        Password.Create("Revenda2026").ToString().Should().NotContain("Revenda");
    }
}

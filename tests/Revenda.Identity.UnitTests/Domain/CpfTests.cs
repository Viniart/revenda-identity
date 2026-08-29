using FluentAssertions;
using Revenda.Identity.Domain.Exceptions;
using Revenda.Identity.Domain.ValueObjects;

namespace Revenda.Identity.UnitTests.Domain;

public class CpfTests
{
    [Theory]
    [InlineData("52998224725")]
    [InlineData("529.982.247-25")]
    [InlineData(" 529 982 247 25 ")]
    public void Create_DeveNormalizarParaSomenteDigitos_QuandoCpfEValido(string input)
    {
        var cpf = Cpf.Create(input);

        cpf.Value.Should().Be("52998224725");
    }

    [Theory]
    [InlineData("52998224724")]
    [InlineData("11111111111")]
    [InlineData("123456789")]
    [InlineData("")]
    [InlineData(null)]
    public void Create_DeveLancarExcecao_QuandoCpfEInvalido(string? input)
    {
        var criar = () => Cpf.Create(input);

        criar.Should().Throw<InvalidCustomerDataException>();
    }

    [Fact]
    public void ToFormatted_DeveAplicarFormatoDeExibicao()
    {
        Cpf.Create("52998224725").ToFormatted().Should().Be("529.982.247-25");
    }

    [Fact]
    public void Igualdade_DeveConsiderarApenasOsDigitos()
    {
        Cpf.Create("529.982.247-25").Should().Be(Cpf.Create("52998224725"));
    }
}

using Revenda.Identity.Domain.Exceptions;

namespace Revenda.Identity.Domain.ValueObjects;

public sealed record Cpf
{
    private const int Length = 11;

    private Cpf(string value) => Value = value;

    /// <summary>Somente dígitos, sem máscara.</summary>
    public string Value { get; }

    public static Cpf Create(string? input)
    {
        var digits = KeepDigits(input);

        if (digits.Length != Length || HasSingleRepeatedDigit(digits) || !HasValidCheckDigits(digits))
        {
            throw new InvalidCustomerDataException("CPF inválido.");
        }

        return new Cpf(digits);
    }

    public string ToMasked() =>
        $"{Value[..3]}.{Value[3..6]}.{Value[6..9]}-{Value[9..]}";

    public override string ToString() => Value;

    private static string KeepDigits(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        return string.Concat(input.Where(char.IsAsciiDigit));
    }

    private static bool HasSingleRepeatedDigit(string digits) =>
        digits.All(digit => digit == digits[0]);

    private static bool HasValidCheckDigits(string digits)
    {
        var first = CalculateCheckDigit(digits, 9);
        var second = CalculateCheckDigit(digits, 10);

        return digits[9] == first && digits[10] == second;
    }

    private static char CalculateCheckDigit(string digits, int take)
    {
        var weight = take + 1;
        var sum = 0;

        for (var i = 0; i < take; i++)
        {
            sum += (digits[i] - '0') * weight--;
        }

        var remainder = sum % Length;
        var checkDigit = remainder < 2 ? 0 : Length - remainder;

        return (char)('0' + checkDigit);
    }
}

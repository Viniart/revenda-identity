using Revenda.Identity.Domain.Exceptions;

namespace Revenda.Identity.Domain.ValueObjects;

/// <summary>
/// Senha em texto puro validada contra a política de acesso. Nunca é persistida nem serializada:
/// serve apenas de passagem entre o caso de uso e o algoritmo de hash.
/// </summary>
public sealed class Password
{
    public const int MinLength = 8;
    public const int MaxLength = 72;

    private Password(string value) => Value = value;

    public string Value { get; }

    public static Password Create(string? input)
    {
        var value = input ?? string.Empty;

        if (value.Length < MinLength || value.Length > MaxLength)
        {
            throw new InvalidCustomerDataException(
                $"A senha deve ter entre {MinLength} e {MaxLength} caracteres.");
        }

        if (!value.Any(char.IsLetter) || !value.Any(char.IsDigit))
        {
            throw new InvalidCustomerDataException("A senha deve conter ao menos uma letra e um número.");
        }

        return new Password(value);
    }

    public override string ToString() => "********";
}

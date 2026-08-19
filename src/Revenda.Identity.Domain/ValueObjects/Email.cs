using System.Net.Mail;
using Revenda.Identity.Domain.Exceptions;

namespace Revenda.Identity.Domain.ValueObjects;

public sealed record Email
{
    public const int MaxLength = 254;

    private Email(string value) => Value = value;

    public string Value { get; }

    public static Email Create(string? input)
    {
        var normalized = input?.Trim().ToLowerInvariant() ?? string.Empty;

        if (normalized.Length is 0 or > MaxLength || !IsWellFormed(normalized))
        {
            throw new InvalidCustomerDataException("E-mail inválido.");
        }

        return new Email(normalized);
    }

    /// <summary>
    /// Usado no login, onde um e-mail malformado é credencial inválida e não erro de validação.
    /// </summary>
    public static bool TryCreate(string? input, out Email email)
    {
        try
        {
            email = Create(input);
            return true;
        }
        catch (InvalidCustomerDataException)
        {
            email = null!;
            return false;
        }
    }

    public override string ToString() => Value;

    private static bool IsWellFormed(string candidate)
    {
        // MailAddress aceita formatos como "Nome <a@b.com>", que não servem como identificador de login.
        if (!MailAddress.TryCreate(candidate, out var address))
        {
            return false;
        }

        return address.Address == candidate && candidate.Count(c => c == '@') == 1;
    }
}

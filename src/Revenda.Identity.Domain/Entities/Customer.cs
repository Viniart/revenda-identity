using Revenda.Identity.Domain.Enums;
using Revenda.Identity.Domain.Exceptions;
using Revenda.Identity.Domain.ValueObjects;

namespace Revenda.Identity.Domain.Entities;

public sealed class Customer
{
    public const int MaxNameLength = 150;

    private Customer()
    {
    }

    private Customer(string name, Cpf cpf, Email email, string passwordHash, CustomerRole role, DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        Name = name;
        Cpf = cpf;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = null!;

    public Cpf Cpf { get; private set; } = null!;

    public Email Email { get; private set; } = null!;

    public string PasswordHash { get; private set; } = null!;

    public CustomerRole Role { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static Customer Register(string? name, Cpf cpf, Email email, string passwordHash, DateTimeOffset now) =>
        new(NormalizeName(name), cpf, email, RequireHash(passwordHash), CustomerRole.Buyer, now);

    public static Customer RegisterAdministrator(
        string? name,
        Cpf cpf,
        Email email,
        string passwordHash,
        DateTimeOffset now) =>
        new(NormalizeName(name), cpf, email, RequireHash(passwordHash), CustomerRole.Administrator, now);

    public void ChangeProfile(string? name, Email email, DateTimeOffset now)
    {
        Name = NormalizeName(name);
        Email = email;
        UpdatedAt = now;
    }

    public void ChangePassword(string passwordHash, DateTimeOffset now)
    {
        PasswordHash = RequireHash(passwordHash);
        UpdatedAt = now;
    }

    private static string NormalizeName(string? name)
    {
        var normalized = name?.Trim() ?? string.Empty;

        if (normalized.Length < 2 || normalized.Length > MaxNameLength)
        {
            throw new InvalidCustomerDataException($"O nome deve ter entre 2 e {MaxNameLength} caracteres.");
        }

        return normalized;
    }

    private static string RequireHash(string passwordHash) =>
        string.IsNullOrWhiteSpace(passwordHash)
            ? throw new InvalidCustomerDataException("O hash da senha é obrigatório.")
            : passwordHash;
}

using Microsoft.AspNetCore.Identity;
using Revenda.Identity.Application.Ports.Output;
using Revenda.Identity.Domain.Entities;
using Revenda.Identity.Domain.ValueObjects;

namespace Revenda.Identity.Infrastructure.Security;

internal sealed class Pbkdf2PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<Customer> _hasher = new();

    public string Hash(Password password) => _hasher.HashPassword(user: null!, password.Value);

    public bool Verify(string passwordHash, string providedPassword)
    {
        try
        {
            var result = _hasher.VerifyHashedPassword(user: null!, passwordHash, providedPassword);
            return result is PasswordVerificationResult.Success
                or PasswordVerificationResult.SuccessRehashNeeded;
        }
        catch (FormatException)
        {
            // Hash corrompido ou fora do formato esperado equivale a credencial inválida.
            return false;
        }
    }
}

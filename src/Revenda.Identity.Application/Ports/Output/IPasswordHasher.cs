using Revenda.Identity.Domain.ValueObjects;

namespace Revenda.Identity.Application.Ports.Output;

public interface IPasswordHasher
{
    string Hash(Password password);

    bool Verify(string passwordHash, string providedPassword);
}

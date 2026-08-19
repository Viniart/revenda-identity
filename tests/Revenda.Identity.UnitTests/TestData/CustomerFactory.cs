using Revenda.Identity.Domain.Entities;
using Revenda.Identity.Domain.ValueObjects;

namespace Revenda.Identity.UnitTests.TestData;

internal static class CustomerFactory
{
    public static readonly DateTimeOffset Agora = new(2026, 3, 10, 12, 0, 0, TimeSpan.Zero);

    public static Customer Comprador(
        string name = "Ana Silva",
        string cpf = "52998224725",
        string email = "ana@revenda.com",
        string passwordHash = "hash-armazenado") =>
        Customer.Register(name, Cpf.Create(cpf), Email.Create(email), passwordHash, Agora);
}

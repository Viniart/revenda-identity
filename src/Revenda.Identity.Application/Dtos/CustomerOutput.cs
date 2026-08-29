using Revenda.Identity.Domain.Entities;

namespace Revenda.Identity.Application.Dtos;

public sealed record CustomerOutput(
    Guid Id,
    string Name,
    string Cpf,
    string Email,
    string Role,
    DateTimeOffset CreatedAt)
{
    public static CustomerOutput From(Customer customer) =>
        new(
            customer.Id,
            customer.Name,
            customer.Cpf.ToFormatted(),
            customer.Email.Value,
            customer.Role.ToString(),
            customer.CreatedAt);
}

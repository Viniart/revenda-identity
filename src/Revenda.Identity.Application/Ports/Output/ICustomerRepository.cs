using Revenda.Identity.Domain.Entities;
using Revenda.Identity.Domain.ValueObjects;

namespace Revenda.Identity.Application.Ports.Output;

public interface ICustomerRepository
{
    Task<Customer?> FindByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Customer?> FindByEmailAsync(Email email, CancellationToken cancellationToken);

    Task<bool> ExistsByEmailAsync(Email email, Guid? ignoredCustomerId, CancellationToken cancellationToken);

    Task<bool> ExistsByCpfAsync(Cpf cpf, CancellationToken cancellationToken);

    Task AddAsync(Customer customer, CancellationToken cancellationToken);
}

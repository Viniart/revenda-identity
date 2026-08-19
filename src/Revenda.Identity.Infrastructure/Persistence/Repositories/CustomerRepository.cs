using Microsoft.EntityFrameworkCore;
using Revenda.Identity.Application.Ports.Output;
using Revenda.Identity.Domain.Entities;
using Revenda.Identity.Domain.ValueObjects;
using Revenda.Identity.Infrastructure.Persistence.Context;

namespace Revenda.Identity.Infrastructure.Persistence.Repositories;

internal sealed class CustomerRepository : ICustomerRepository
{
    private readonly IdentityDbContext _context;

    public CustomerRepository(IdentityDbContext context) => _context = context;

    public Task<Customer?> FindByIdAsync(Guid id, CancellationToken cancellationToken) =>
        _context.Customers.FirstOrDefaultAsync(customer => customer.Id == id, cancellationToken);

    public Task<Customer?> FindByEmailAsync(Email email, CancellationToken cancellationToken) =>
        _context.Customers.FirstOrDefaultAsync(customer => customer.Email == email, cancellationToken);

    public Task<bool> ExistsByEmailAsync(Email email, Guid? ignoredCustomerId, CancellationToken cancellationToken) =>
        _context.Customers.AnyAsync(
            customer => customer.Email == email && (ignoredCustomerId == null || customer.Id != ignoredCustomerId),
            cancellationToken);

    public Task<bool> ExistsByCpfAsync(Cpf cpf, CancellationToken cancellationToken) =>
        _context.Customers.AnyAsync(customer => customer.Cpf == cpf, cancellationToken);

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken) =>
        await _context.Customers.AddAsync(customer, cancellationToken);
}

using Revenda.Identity.Application.Ports.Output;
using Revenda.Identity.Infrastructure.Persistence.Context;

namespace Revenda.Identity.Infrastructure.Persistence;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly IdentityDbContext _context;

    public UnitOfWork(IdentityDbContext context) => _context = context;

    public Task CommitAsync(CancellationToken cancellationToken) =>
        _context.SaveChangesAsync(cancellationToken);
}

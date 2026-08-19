namespace Revenda.Identity.Application.Ports.Output;

public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken cancellationToken);
}

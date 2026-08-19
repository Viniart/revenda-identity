namespace Revenda.Identity.Application.Ports.Output;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}

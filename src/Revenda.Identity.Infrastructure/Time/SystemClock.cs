using Revenda.Identity.Application.Ports.Output;

namespace Revenda.Identity.Infrastructure.Time;

internal sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}

using Revenda.Identity.Application.Ports.Output;

namespace Revenda.Identity.UnitTests.TestData;

internal sealed class FixedClock : IClock
{
    public FixedClock(DateTimeOffset instant) => UtcNow = instant;

    public DateTimeOffset UtcNow { get; }
}

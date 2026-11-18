using System.Diagnostics.CodeAnalysis;
using HookRelay.Api.Destinations;

namespace HookRelay.Tests.Destinations;

public sealed class FailingDestinationStore : IDestinationStore
{
    public bool TryGet(string id, [NotNullWhen(true)] out Destination? destination)
    {
        destination = null;
        return false;
    }
    public bool TryGetByUrl(string absoluteUrl, [NotNullWhen(true)] out Destination? d)
    {
        d = null;
        return false;
    }
    
    public IReadOnlyList<Destination> GetDestinationsBySource(SourceType? sourceType)
    {
        throw new NotImplementedException();
    }
    public void Update(Destination destination)
    {
        throw new NotImplementedException();
    }

    public bool TryAdd(Destination destination, out Destination stored)
    {
        throw new IOException("disk full");
    }
}

// For the "unknown failure" test: throws something that is NOT ours (not mapped),
// with a message that must never reach a client.
public sealed class ExplodingDestinationStore : IDestinationStore
{
    public bool TryGet(string id, [NotNullWhen(true)] out Destination? destination)
    {
        destination = null;
        return false;
    }

    public bool TryGetByUrl(string absoluteUrl, [NotNullWhen(true)] out Destination? d)
    {
        d = null;
        return false;
    }

    public IReadOnlyList<Destination> GetDestinationsBySource(SourceType? sourceType) =>
        throw new NotImplementedException();

    public void Update(Destination destination) => throw new NotImplementedException();

    public bool TryAdd(Destination destination, out Destination stored)
    {
        throw new InvalidOperationException("db password=hunter2 at 10.0.0.5");
    }
}

using System.Diagnostics.CodeAnalysis;
using HookRelay.Api.Destinations;

namespace HookRelay.Tests.Destinations;

public sealed class FailingDestinationStore : IDestinationStore
{
    public void Add(Destination destination)
    {
        throw new IOException("disk full");
    }

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
}
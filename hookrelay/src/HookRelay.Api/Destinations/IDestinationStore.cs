using System.Diagnostics.CodeAnalysis;

namespace HookRelay.Api.Destinations;

public interface IDestinationStore
{
    bool TryGet(string id, [NotNullWhen(true)] out Destination? destination);
    bool TryGetByUrl(string absoluteUrl, [NotNullWhen(true)] out Destination? d);
    IReadOnlyList<Destination> GetDestinationsBySource(SourceType? sourceType);
    void Update(Destination destination);
    bool TryAdd(Destination destination, out Destination stored);
}

public sealed class InMemoryDestinationStore : IDestinationStore
{
    private readonly Dictionary<string, Destination> _destination = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _idByUrl = new(StringComparer.Ordinal);
    private readonly List<Destination> _listDestination = new();
    private readonly object _gate = new();

    public bool TryGet(string id, [NotNullWhen(true)] out Destination? destination)
    {
        lock (_gate)
        {
            if(string.IsNullOrWhiteSpace(id)) 
            {
                destination = null; return false;
            }
            return _destination.TryGetValue(id, out destination);
        }
    }

    public IReadOnlyList<Destination> GetDestinationsBySource(SourceType? sourceType)
    {
        lock (_gate)
        {
            return _listDestination
                .Where(d => sourceType is null || d.Source == sourceType)
                .ToList();
        }
    }

    public void Update(Destination destination)
    {
        lock (_gate)
        {
            _destination[destination.Id] = destination;
            var index = _listDestination.FindIndex(x => x.Id == destination.Id);
            _listDestination[index] = destination;
        }
    }

    public bool TryGetByUrl(string absoluteUrl, [NotNullWhen(true)] out Destination? destination)
    {
        lock (_gate)
        {
            if (string.IsNullOrWhiteSpace(absoluteUrl))
            {
                destination = null; return false;
            }
            destination = null;
            return _idByUrl.TryGetValue(absoluteUrl, out var id)      // 1. URL → id
                && _destination.TryGetValue(id, out destination);     // 2. id → the one real object
        }
    }

    public bool TryAdd(Destination destination, out Destination stored)
    {
        lock (_gate)
        {
            if (_idByUrl.TryGetValue(destination.Url.AbsoluteUri, out var existingId))
            {
                stored = _destination[existingId];
                return false;                 // URL taken
            }
            // add to all three collections here
            _destination[destination.Id] = destination;
            _idByUrl[destination.Url.AbsoluteUri] = destination.Id;
            _listDestination.Add(destination);
            stored = destination;
            return true;
        }
    }
}
using System.Diagnostics.CodeAnalysis;

namespace HookRelay.Api.Destinations;

public interface IDestinationStore
{
    bool TryGet(string id, [NotNullWhen(true)] out Destination? destination);
    bool TryGetByUrl(string absoluteUrl, [NotNullWhen(true)] out Destination? d);
    void Add(Destination destination);
    IReadOnlyList<Destination> GetDestinationsBySource(SourceType? sourceType);
    void Update(Destination destination);
}

public sealed class InMemoryDestinationStore : IDestinationStore
{
    private readonly Dictionary<string, Destination> _destination = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _idByUrl = new(StringComparer.Ordinal);
    private readonly List<Destination> _listDestination = new();

    public bool TryGet(string id, [NotNullWhen(true)] out Destination? destination)
    {
        if(string.IsNullOrWhiteSpace(id)) 
        {
            destination = null; return false;
        }
        return _destination.TryGetValue(id, out destination);
    }

    public void Add(Destination destination)
    {
        _destination[destination.Id] = destination;
        _idByUrl[destination.Url.AbsoluteUri] = destination.Id;
        _listDestination.Add(destination);
    }

    public IReadOnlyList<Destination> GetDestinationsBySource(SourceType? sourceType)
    {
        return _listDestination.Where(x => sourceType == null || x.Source == sourceType).ToList();
    }

    public void Update(Destination destination)
    {
        _destination[destination.Id] = destination;
        var index = _listDestination.FindIndex(x => x.Id == destination.Id);
        _listDestination[index] = destination;
    }

    public bool TryGetByUrl(string absoluteUrl, [NotNullWhen(true)] out Destination? destination)
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
using System.Diagnostics.CodeAnalysis;

namespace HookRelay.Api.Destinations;

public sealed class DestinationService
{
    private readonly IDestinationStore _store;
    private readonly IIdGenerator _idGenerator;
    private readonly ISecretGenerator _secretGenerator;

    public DestinationService(IDestinationStore store, IIdGenerator idGenerator, ISecretGenerator secretGenerator)
    {
        _store = store;
        _idGenerator = idGenerator;
        _secretGenerator = secretGenerator;
    }
    public Destination Create(string name, string url, string source)
    {
        var errors = new Dictionary<string, List<string>>();

        string? trimmed = ValidateName(name, errors);    // all three always run,
        Uri? uri = ValidateUrl(url, errors);      // each stops at its own
        SourceType? src = ParseSource(source, errors);   // first problem

        ThrowIfAny(errors);

        // from here every value is valid, so '!' is a fact, not a hope
        if (_store.TryGetByUrl(uri!.AbsoluteUri, out var d))
            throw new DuplicateDestinationException(d.Url.AbsoluteUri, d.Id, d.Status);

        string id = _idGenerator.Next("dst");
        string secret = _secretGenerator.NewSecret();
        var destination = new Destination(id, trimmed!, uri, src!.Value, secret, DestinationStatus.Active);
        try
        {
            _store.Add(destination);
        }
        catch (IOException ex)                       // narrow: only the failure you expect
        {
            throw new DestinationStoreUnavailableException(ex);   // wrap, keep the cause
        }

        return destination;
    }

    private static string? ValidateName(string? name, Dictionary<string, List<string>> errors)
    {
        if (string.IsNullOrWhiteSpace(name)) { Fail("name", "Name is required.", errors); return null; }
        string trimmed = name.Trim();
        if (trimmed.Length > 100) { Fail("name", "Name must be 1 to 100 characters.", errors); return null; }
        return trimmed;
    }

    private static Uri? ValidateUrl(string? url, Dictionary<string, List<string>> errors)
    {
        if (string.IsNullOrWhiteSpace(url)) { Fail("url", "Url is required.", errors); return null; }
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) { Fail("url", "Url must be an absolute http(s) URL.", errors); return null; }
        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
        { Fail("url", "Url scheme must be http or https.", errors); return null; }
        if (uri.Scheme == Uri.UriSchemeHttp && uri.Host != "localhost")
        { Fail("url", "http is only allowed for localhost.", errors); return null; }
        if (uri.HostNameType != UriHostNameType.Dns) { Fail("url", "Url must use a host name, not an IP address.", errors); return null; }
        if (!string.IsNullOrEmpty(uri.UserInfo)) { Fail("url", "Url must not contain credentials.", errors); return null; }
        return uri;
    }

    private static SourceType? ParseSource(string? source, Dictionary<string, List<string>> errors)
    {
        if (string.IsNullOrWhiteSpace(source)) { Fail("source", "Source is required.", errors); return null; }
        string text = source.Trim();
        if (!text.All(char.IsLetter)
            || !Enum.TryParse<SourceType>(text, ignoreCase: true, out var value)
            || !Enum.IsDefined(value)) { Fail("source", "Source must be stripe, github or generic.", errors); return null; }
        return value;
    }

    private static void Fail(string field, string message, Dictionary<string, List<string>> errors)
    {
        if (!errors.TryGetValue(field, out var list))
            errors[field] = list = new List<string>();
        list.Add(message);
    }

    private static void ThrowIfAny(Dictionary<string, List<string>> errors)
    {
        if (errors.Count > 0)
            throw new DestinationValidationException(
                errors.ToDictionary(e => e.Key, e => e.Value.ToArray()));
    }

    public bool TryGet(string id, [NotNullWhen(true)] out Destination? destination)
    {
        return _store.TryGet(id, out destination);
    }

    public IReadOnlyList<Destination> GetDestinations(string? source)
    {
        SourceType? sourceValue = null;
        if (!string.IsNullOrWhiteSpace(source))
        {
            var errors = new Dictionary<string, List<string>>();
            sourceValue = ParseSource(source, errors);
            ThrowIfAny(errors);
        }
        return _store.GetDestinationsBySource(sourceValue);
    }

    public Destination DisableDestination(string id)
    {
        if(!_store.TryGet(id, out Destination? destination))
        {
            throw new DestinationNotFoundException(id);
        }

        if (destination.Status == DestinationStatus.Disabled)
        {
            throw new DestinationAlreadyDisabledException(id);
        }

        var disabled = destination with { Status = DestinationStatus.Disabled };

        _store.Update(disabled);
        return disabled;
    }
}
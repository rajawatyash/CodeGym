namespace HookRelay.Api.Destinations;

public sealed class DuplicateDestinationException : InvalidOperationException
{
    public DuplicateDestinationException(string url, string existingId, DestinationStatus existingStatus)
        : base($"URL '{url}' is already registered as {existingId} ({existingStatus}).")
    {
        Url = url;
        ExistingId = existingId;
        ExistingStatus = existingStatus;
    }

    public string Url { get; }
    public string ExistingId { get; }
    public DestinationStatus ExistingStatus { get; }
}

public sealed class DestinationAlreadyDisabledException : InvalidOperationException
{
    public DestinationAlreadyDisabledException(string id)
        : base($"Id '{id}' already disabled.") => Id = id;

    public string Id { get; }
}

public sealed class DestinationValidationException : ArgumentException
{
    public DestinationValidationException(Dictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }

    public IDictionary<string, string[]> Errors { get; }
}

public sealed class DestinationNotFoundException : KeyNotFoundException
{
    public DestinationNotFoundException(string id)
        : base($"Id '{id}' not found.")
        {
            Id = id;
        }

    public string Id { get; }
}

public sealed class DestinationStoreUnavailableException : Exception
{
    public DestinationStoreUnavailableException(Exception inner)
        : base("The destination store is unavailable.", inner) { }   // safe message; cause kept
}

public sealed class EventValidationException : InvalidOperationException
{
    public EventValidationException(Dictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }

    public IDictionary<string, string[]> Errors { get; }
}
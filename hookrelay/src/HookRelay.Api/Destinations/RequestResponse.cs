namespace HookRelay.Api.Destinations;

public record CreateDestinationRequest(string Name, string Url, string Source);

public record CreateDestinationResponse(string Id, string Name, string Url, string Source, string SigningSecret, string Status, DateTimeOffset CreatedAt)
{
    public static CreateDestinationResponse From(Destination destination) => new(destination.Id, destination.Name, destination.Url.AbsoluteUri, destination.Source.ToString(), destination.SigningSecret, destination.Status.ToString(), destination.CreatedAt);
}


public record GetDestinationResponse(string Id, string Name, string Url, string Source, string Status, DateTimeOffset CreatedAt)
{
    public static GetDestinationResponse From(Destination destination) => new(destination.Id, destination.Name, destination.Url.AbsoluteUri, destination.Source.ToString(), destination.Status.ToString(), destination.CreatedAt);
}
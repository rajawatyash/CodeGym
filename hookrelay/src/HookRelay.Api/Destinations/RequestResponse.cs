namespace HookRelay.Api.Destinations;

public record CreateDestinationRequest(string Name, string Url, string Source);

public record CreateDestinationResponse(string Id, string Name, string Url, string Source, string SigningSecret, string Status)
{
    public static CreateDestinationResponse From(Destination destination) => new(destination.Id, destination.Name, destination.Url.AbsoluteUri, destination.Source.ToString(), destination.SigningSecret, destination.Status.ToString());
}


public record GetDestinationResponse(string Id, string Name, string Url, string Source, string Status)
{
    public static GetDestinationResponse From(Destination destination) => new(destination.Id, destination.Name, destination.Url.AbsoluteUri, destination.Source.ToString(), destination.Status.ToString());
}
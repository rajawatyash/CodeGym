namespace HookRelay.Api.Destinations;


public enum SourceType { Stripe, GitHub, Generic }
public enum DestinationStatus { Active, Disabled }
public record Destination(string Id, string Name, Uri Url, SourceType Source,
                           string SigningSecret, DestinationStatus Status, DateTimeOffset CreatedAt);
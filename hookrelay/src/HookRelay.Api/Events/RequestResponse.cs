namespace HookRelay.Api.Events;

public record EventResponse(string Id, string Source, string EventId, string Status, DateTimeOffset ReceivedAt)
{
    public static EventResponse From(WebhookEvent webHookEvent) => new(webHookEvent.Id, webHookEvent.Key.Source.ToString(), webHookEvent.Key.EventId, webHookEvent.Status.ToString(), webHookEvent.ReceivedAt);
}
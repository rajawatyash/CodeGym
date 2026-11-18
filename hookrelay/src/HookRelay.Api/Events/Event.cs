using HookRelay.Api.Destinations;

namespace HookRelay.Api.Events;

public enum EventStatus { Received }

public record struct EventKey(SourceType Source, string EventId);

public record WebhookEvent(string Id, EventKey Key, string Payload, DateTimeOffset ReceivedAt, EventStatus Status);
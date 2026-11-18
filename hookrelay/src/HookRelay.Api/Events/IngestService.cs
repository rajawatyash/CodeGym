using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using HookRelay.Api.Destinations;

namespace HookRelay.Api.Events;

public sealed record IngestResult(WebhookEvent Event, bool IsNew);

public class IngestService
{
    public static int BuildCount;
    private readonly ConcurrentDictionary<EventKey, Lazy<string>> _summaries = new();
    private readonly IEventStore _store;
    private readonly IIdGenerator _ids;
    private readonly TimeProvider _time;

    public IngestService(IEventStore store, IIdGenerator ids, TimeProvider time)
    {
        _store = store;
        _ids = ids;
        _time = time;
    }
    public IngestResult Ingest(string source, string? webhookId, string payload)
    {
        // 1. Check everything first, collect the problems, throw once
        var errors = new Dictionary<string, string[]>();
        if (!TryParseSource(source, out var sourceType))
            errors["source"] = [$"Unknown source '{source}'."];
        // blank webhookId → errors["webhookId"]
        if(string.IsNullOrWhiteSpace(webhookId))
            errors["webhookId"] = [$"WebHookId is required."];
        // blank payload   → errors["payload"]
        if(string.IsNullOrWhiteSpace(payload))
            errors["payload"] = [$"Payload is required."];
        if (errors.Count > 0)
            throw new EventValidationException(errors);

        // 2. build the EventKey from sourceType and webhookId
        var eventKey = new EventKey(sourceType, webhookId!);
        // 3. build a new WebhookEvent: a new id from your IIdGenerator,
        var webHookEvent = new WebhookEvent(_ids.Next("ev"), eventKey, payload, _time.GetUtcNow(), EventStatus.Received);
        bool isNew = _store.TryAdd(webHookEvent, out var stored);
        return new IngestResult(stored, isNew);
    }

    public bool TryGet(string id, [NotNullWhen(true)] out WebhookEvent? webhookEvent)
    {
        return _store.TryGet(id, out webhookEvent);
    }
    public IReadOnlyList<WebhookEvent> GetWebhookEvents(string? source)
    {
        if (string.IsNullOrWhiteSpace(source))
            return _store.List(null);

        if (!TryParseSource(source, out var sourceType))
            throw new EventValidationException(new Dictionary<string, string[]>
            {
                ["source"] = [$"Unknown source '{source}'."]
            });

        return _store.List(sourceType);
    }

    public string Summary(EventKey key) => _summaries.GetOrAdd(key, k => new Lazy<string>(() => BuildSummary(k))).Value;

    private string BuildSummary(EventKey k)
    {
        Interlocked.Increment(ref BuildCount);
        Thread.Sleep(50);

        return $"{k.Source}:{k.EventId}";
    }

    private static bool TryParseSource(string text, out SourceType source) =>
        Enum.TryParse(text, ignoreCase: true, out source) && Enum.IsDefined(source);

}
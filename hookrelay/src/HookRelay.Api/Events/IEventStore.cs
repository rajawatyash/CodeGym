using System.Collections.Concurrent;
using HookRelay.Api.Destinations;

namespace HookRelay.Api.Events;

public interface IEventStore
{
    bool TryAdd(WebhookEvent ev, out WebhookEvent stored);
    bool TryGet(string id, out WebhookEvent? ev);
    IReadOnlyList<WebhookEvent> List(SourceType? source);
}

public sealed class InMemoryEventStore : IEventStore
{
    private readonly ConcurrentDictionary<EventKey, WebhookEvent> _webhookDictionary = new();
    private readonly ConcurrentDictionary<string, WebhookEvent> _webHookById = new();

    public bool TryAdd(WebhookEvent ev, out WebhookEvent stored)
    {
        stored = _webhookDictionary.GetOrAdd(ev.Key, ev);   // atomic: theirs or ours
        if(!ReferenceEquals(stored, ev))
        {
            return false;
        }
        _webHookById.TryAdd(ev.Id, ev);
        return true;
    }

    public bool TryGet(string id, out WebhookEvent? ev)
    {
        if(_webHookById.TryGetValue(id, out ev))
        {
            return true;
        }
        return false;
    }

    public IReadOnlyList<WebhookEvent> List(SourceType? source)
    {
        return _webhookDictionary
            .Select(pair => pair.Value)
            .Where(ev => source is null || ev.Key.Source == source)
            .OrderByDescending(ev => ev.ReceivedAt)
            .Take(100)
            .ToList();
    }
}
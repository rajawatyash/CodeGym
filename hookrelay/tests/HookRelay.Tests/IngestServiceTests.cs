using System.Collections.Concurrent;
using HookRelay.Api.Events;
using HookRelay.Tests.Destinations;
using Microsoft.Extensions.Time.Testing;

namespace HookRelay.Tests.Events;

public class IngestServiceTests
{
    private static readonly DateTimeOffset Start = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private static IngestService NewService() =>
        new(new InMemoryEventStore(), new FakeIdGenerator(), new FakeTimeProvider(Start));


    [Fact]
    public void Parallel_events_of_one_store_exactly_one()
    {
        var service = NewService();
        var results = new ConcurrentBag<IngestResult>();     // thread-safe collection

        Parallel.For(0, 200, _ => results.Add(service.Ingest("stripe", "evt_1", "{}")));

        Assert.Single(results, r => r.IsNew);                        // exactly one stored it
        Assert.Single(results.Select(r => r.Event.Id).Distinct());   // everyone got the same event
    }

    [Fact]
    public void Summary_called_in_parallel_for_same_key_builds_once()
    {
        var service = NewService();
        IngestService.BuildCount = 0;

        var eventKey = new EventKey(Api.Destinations.SourceType.Stripe, "ev_1");
        Parallel.For(0, 20, _ => service.Summary(eventKey));

        Assert.Equal(1, IngestService.BuildCount);
    }
}
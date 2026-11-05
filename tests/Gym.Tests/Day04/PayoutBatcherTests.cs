using Gym.Day04;

namespace Gym.Tests.Day04;

public class PayoutBatcherTests
{
    private static string[] Ids(List<List<Payout>> batches) =>
        batches.Select(b => string.Join(",", b.Select(p => p.Id))).ToArray();

    [Fact]
    public void Starts_a_new_batch_on_count_or_total()
    {
        var payouts = new[]
        {
            new Payout("P1", 40m), new Payout("P2", 50m), new Payout("P3", 20m),
            new Payout("P4", 5m), new Payout("P5", 5m), new Payout("P6", 90m),
        };

        var result = PayoutBatcher.Batch(payouts, 3, 100m);

        Assert.Equal(new[] { "P1,P2", "P3,P4,P5", "P6" }, Ids(result));
    }

    [Fact]
    public void Exactly_the_max_total_fits()
    {
        var result = PayoutBatcher.Batch(new[] { new Payout("A", 60m), new Payout("B", 40m) }, 10, 100m);

        Assert.Equal(new[] { "A,B" }, Ids(result));
    }

    [Fact]
    public void Max_count_one_gives_one_payout_per_batch()
    {
        var result = PayoutBatcher.Batch(new[] { new Payout("A", 1m), new Payout("B", 1m) }, 1, 100m);

        Assert.Equal(new[] { "A", "B" }, Ids(result));
    }

    [Fact]
    public void No_payouts_gives_no_batches() =>
        Assert.Empty(PayoutBatcher.Batch(Array.Empty<Payout>(), 3, 100m));

    [Fact]
    public void Payout_bigger_than_max_total_throws() =>
        Assert.Throws<ArgumentException>(() => PayoutBatcher.Batch(new[] { new Payout("A", 101m) }, 3, 100m));

    [Fact]
    public void Zero_payout_throws() =>
        Assert.Throws<ArgumentException>(() => PayoutBatcher.Batch(new[] { new Payout("A", 0m) }, 3, 100m));

    [Fact]
    public void Bad_limits_throw()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PayoutBatcher.Batch(Array.Empty<Payout>(), 0, 100m));
        Assert.Throws<ArgumentOutOfRangeException>(() => PayoutBatcher.Batch(Array.Empty<Payout>(), 3, 0m));
    }

    [Fact]
    public void Null_throws() =>
        Assert.Throws<ArgumentNullException>(() => PayoutBatcher.Batch(null!, 3, 100m));
}
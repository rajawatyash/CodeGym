using Gym.Day02;

namespace Gym.Tests.Day02;

public class DailyTotalsTests
{
    private static readonly DateOnly Oct1 = new(2026, 10, 1);
    private static readonly DateOnly Oct2 = new(2026, 10, 2);
    private static readonly DateOnly Oct3 = new(2026, 10, 3);

    [Fact]
    public void Adds_up_each_day_and_sorts_by_day()
    {
        var payments = new[]
        {
            new Payment(Oct2, 10m),
            new Payment(Oct1, 5m),
            new Payment(Oct2, -3m),
            new Payment(Oct1, 20m),
        };

        var result = Statement.DailyTotals(payments);

        Assert.Equal(new[] { (Oct1, 25m), (Oct2, 7m) }, result.ToArray());
    }

    [Fact]
    public void A_day_that_nets_to_zero_still_appears()
    {
        var payments = new[]
        {
            new Payment(Oct3, 40m),
            new Payment(Oct3, -40m),
            new Payment(Oct1, 1m),
        };

        var result = Statement.DailyTotals(payments);

        Assert.Equal(new[] { (Oct1, 1m), (Oct3, 0m) }, result.ToArray());
    }

    [Fact]
    public void No_payments_gives_empty_list() =>
        Assert.Empty(Statement.DailyTotals(Array.Empty<Payment>()));

    [Fact]
    public void Null_throws() =>
        Assert.Throws<ArgumentNullException>(() => Statement.DailyTotals(null!));
}
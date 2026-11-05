using Gym.Day05;

namespace Gym.Tests.Day05;

public class BackoffTests
{
    private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);

    [Fact]
    public void Doubles_then_caps()
    {
        var result = Backoff.Delays(5, OneSecond, TimeSpan.FromSeconds(10));

        Assert.Equal(new[] { 1, 2, 4, 8, 10 }, result.Select(d => (int)d.TotalSeconds).ToArray());
    }

    [Fact]
    public void Zero_retries_gives_no_delays() =>
        Assert.Empty(Backoff.Delays(0, OneSecond, TimeSpan.FromSeconds(10)));

    [Fact]
    public void Many_retries_do_not_overflow()
    {
        var max = TimeSpan.FromHours(1);

        var result = Backoff.Delays(100, OneSecond, max);

        Assert.Equal(100, result.Count);
        Assert.All(result, d => Assert.True(d <= max));
        Assert.Equal(max, result[99]);
    }

    [Fact]
    public void Base_equal_to_max_gives_constant_delays()
    {
        var result = Backoff.Delays(3, OneSecond, OneSecond);

        Assert.Equal(new[] { OneSecond, OneSecond, OneSecond }, result.ToArray());
    }

    [Fact]
    public void Bad_arguments_throw()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Backoff.Delays(-1, OneSecond, OneSecond));
        Assert.Throws<ArgumentOutOfRangeException>(() => Backoff.Delays(3, TimeSpan.Zero, OneSecond));
        Assert.Throws<ArgumentException>(() => Backoff.Delays(3, TimeSpan.FromSeconds(5), OneSecond));
    }
}
using Gym.Day03;

namespace Gym.Tests.Day03;

public class BurstDetectorTests
{
    private static readonly DateTime T0 = new(2026, 10, 1, 10, 0, 0);
    private static readonly TimeSpan Minute = TimeSpan.FromSeconds(60);

    private static Charge At(string card, int seconds) => new(card, T0.AddSeconds(seconds), 1m);

    [Fact]
    public void Flags_card_with_too_many_charges_in_the_window()
    {
        var charges = new[] { At("A", 0), At("B", 0), At("A", 20), At("B", 30), At("A", 50), At("B", 60) };

        Assert.Equal(new[] { "A" }, BurstDetector.FlagBursts(charges, 2, Minute).ToArray());
    }

    [Fact]
    public void Exactly_window_apart_is_not_the_same_window()
    {
        var charges = new[] { At("B", 0), At("B", 30), At("B", 60) };

        Assert.Empty(BurstDetector.FlagBursts(charges, 2, Minute));
    }

    [Fact]
    public void Same_time_charges_count()
    {
        var charges = new[] { At("A", 5), At("A", 5), At("A", 5) };

        Assert.Equal(new[] { "A" }, BurstDetector.FlagBursts(charges, 2, Minute).ToArray());
    }

    [Fact]
    public void Each_card_once_and_sorted()
    {
        var charges = new[]
        {
            At("Z", 0), At("M", 1), At("Q", 2), At("Z", 3), At("M", 4),
            At("Z", 5), At("M", 6), At("Z", 7), At("Q", 100),
        };

        Assert.Equal(new[] { "M", "Z" }, BurstDetector.FlagBursts(charges, 2, Minute).ToArray());
    }

    [Fact]
    public void Slow_steady_charges_are_fine()
    {
        var charges = new[] { At("A", 0), At("A", 40), At("A", 80), At("A", 120), At("A", 160) };

        Assert.Empty(BurstDetector.FlagBursts(charges, 2, Minute));
    }

    [Fact]
    public void No_charges_gives_empty_result() =>
        Assert.Empty(BurstDetector.FlagBursts(Array.Empty<Charge>(), 2, Minute));

    [Fact]
    public void Unsorted_input_throws() =>
        Assert.Throws<ArgumentException>(() =>
            BurstDetector.FlagBursts(new[] { At("A", 10), At("A", 0) }, 2, Minute));

    [Fact]
    public void Bad_limits_throw()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => BurstDetector.FlagBursts(Array.Empty<Charge>(), 0, Minute));
        Assert.Throws<ArgumentOutOfRangeException>(() => BurstDetector.FlagBursts(Array.Empty<Charge>(), 2, TimeSpan.Zero));
    }

    [Fact]
    public void Null_throws() =>
        Assert.Throws<ArgumentNullException>(() => BurstDetector.FlagBursts(null!, 2, Minute));
}
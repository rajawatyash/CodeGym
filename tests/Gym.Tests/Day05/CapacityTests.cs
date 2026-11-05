using Gym.Day05;

namespace Gym.Tests.Day05;

public class CapacityTests
{
    [Fact]
    public void Estimates_payflow()
    {
        var result = Capacity.Estimate(new TrafficInput(10_000_000, 10, 2_000, 7, 5));

        Assert.Equal(115.74, result.WritesPerSecond, 2);
        Assert.Equal(578.70, result.PeakWritesPerSecond, 2);
        Assert.Equal(1157.41, result.ReadsPerSecond, 2);
        Assert.Equal(51100, result.StorageGb, 2);
    }

    [Fact]
    public void One_million_a_day_is_about_twelve_a_second() =>
        Assert.Equal(11.57, Capacity.Estimate(new TrafficInput(1_000_000, 0, 1_000, 1, 2)).WritesPerSecond, 2);

    [Fact]
    public void Zero_payments_throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Capacity.Estimate(new TrafficInput(0, 1, 1, 1, 1)));
}
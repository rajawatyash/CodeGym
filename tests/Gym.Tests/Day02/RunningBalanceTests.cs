using Gym.Day02;

namespace Gym.Tests.Day02;

public class RunningBalanceTests
{
    [Fact]
    public void Balance_after_each_movement()
    {
        var result = Ledger.RunningBalance(100m, new[] { -20m, 50m, -30m });

        Assert.Equal(new[] { 80m, 130m, 100m }, result);
    }

    [Fact]
    public void No_movements_gives_empty_array() =>
        Assert.Empty(Ledger.RunningBalance(100m, Array.Empty<decimal>()));

    [Fact]
    public void Null_throws() =>
        Assert.Throws<ArgumentNullException>(() => Ledger.RunningBalance(0m, null!));
}
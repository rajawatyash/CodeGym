using Gym.Day02;

namespace Gym.Tests.Day02;

public class FirstOverdraftTests
{
    [Fact]
    public void Finds_first_index_below_zero() =>
        Assert.Equal(2, Ledger.FirstOverdraft(50m, new[] { -20m, -30m, -10m, 100m }));

    [Fact]
    public void Zero_balance_is_not_an_overdraft() =>
        Assert.Equal(-1, Ledger.FirstOverdraft(10m, new[] { 5m, -15m, 100m }));

    [Fact]
    public void One_cent_below_zero_counts() =>
        Assert.Equal(0, Ledger.FirstOverdraft(0m, new[] { -0.01m }));

    [Fact]
    public void No_movements_means_no_overdraft() =>
        Assert.Equal(-1, Ledger.FirstOverdraft(0m, Array.Empty<decimal>()));

    [Fact]
    public void Null_throws() =>
        Assert.Throws<ArgumentNullException>(() => Ledger.FirstOverdraft(0m, null!));
}
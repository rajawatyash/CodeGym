using Gym.Day04;

namespace Gym.Tests.Day04;

public class FeesTests
{
    [Fact]
    public void Domestic_card() =>
        Assert.Equal((3.20m, 96.80m), Fees.Calculate(100m, false));

    [Fact]
    public void International_card_adds_one_and_a_half_percent() =>
        Assert.Equal((4.70m, 95.30m), Fees.Calculate(100m, true));

    [Fact]
    public void Rounds_to_cents() =>
        Assert.Equal((0.60m, 9.90m), Fees.Calculate(10.50m, false));

    [Fact]
    public void Halves_round_up_not_to_even() =>
        Assert.Equal((0.45m, 4.55m), Fees.Calculate(5.00m, false));

    [Fact]
    public void Custom_pricing_with_named_arguments() =>
        Assert.Equal((3.05m, 196.95m), Fees.Calculate(200m, false, percent: 1.4m, fixedFee: 0.25m));

    [Fact]
    public void Zero_amount_throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Fees.Calculate(0m, false));

    [Fact]
    public void Fee_bigger_than_amount_throws() =>
        Assert.Throws<ArgumentException>(() => Fees.Calculate(0.20m, false));
}
using Gym.Day06;

namespace Gym.Tests.Day06;

public class MoneyParseTests
{
    [Theory]
    [InlineData("USD 12.50", 1250L, Currency.USD)]
    [InlineData("JPY 500", 500L, Currency.JPY)]
    [InlineData("USD -5.00", -500L, Currency.USD)]
    [InlineData("KWD 1.250", 1250L, Currency.KWD)]
    public void Parses_valid_text(string text, long minor, Currency currency)
    {
        Assert.True(Money.TryParse(text, out var money));
        Assert.Equal(Money.FromMinor(minor, currency), money);
    }

    [Theory]
    [InlineData("usd 12.50")]
    [InlineData("1 12.50")]
    [InlineData("USD 12.505")]
    [InlineData("JPY 1.5")]
    [InlineData("USD")]
    [InlineData("USD abc")]
    [InlineData("XXX 1")]
    [InlineData("USD  12.50")]
    [InlineData("")]
    [InlineData(null)]
    public void Rejects_invalid_text(string? text) =>
        Assert.False(Money.TryParse(text, out _));

    [Fact]
    public void Parse_throws_on_invalid_text() =>
        Assert.Throws<FormatException>(() => Money.Parse("usd 1"));
}
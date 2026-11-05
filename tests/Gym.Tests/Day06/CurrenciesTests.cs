using Gym.Day06;

namespace Gym.Tests.Day06;

public class CurrenciesTests
{
    [Theory]
    [InlineData(Currency.USD, 2, 100L)]
    [InlineData(Currency.EUR, 2, 100L)]
    [InlineData(Currency.INR, 2, 100L)]
    [InlineData(Currency.JPY, 0, 1L)]
    [InlineData(Currency.KWD, 3, 1000L)]
    public void Knows_minor_digits(Currency currency, int digits, long factor)
    {
        Assert.Equal(digits, Currencies.MinorDigits(currency));
        Assert.Equal(factor, Currencies.Factor(currency));
    }

    [Fact]
    public void Undefined_value_throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Currencies.MinorDigits((Currency)99));
}
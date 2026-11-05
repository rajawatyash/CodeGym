using Gym.Day06;

namespace Gym.Tests.Day06;

public class MoneyTests
{
    [Theory]
    [InlineData(12.50, Currency.USD, 1250L)]
    [InlineData(500, Currency.JPY, 500L)]
    [InlineData(1.25, Currency.KWD, 1250L)]
    [InlineData(0, Currency.EUR, 0L)]
    public void Stores_minor_units(double amount, Currency currency, long minor) =>
        Assert.Equal(minor, Money.Of((decimal)amount, currency).MinorUnits);

    [Fact]
    public void Too_many_decimals_throws()
    {
        Assert.Throws<ArgumentException>(() => Money.Of(12.505m, Currency.USD));
        Assert.Throws<ArgumentException>(() => Money.Of(1.5m, Currency.JPY));
    }

    [Fact]
    public void Amount_converts_back() =>
        Assert.Equal(12.50m, Money.FromMinor(1250, Currency.USD).Amount);

    [Fact]
    public void Equal_values_are_equal()
    {
        Assert.Equal(Money.Of(12.5m, Currency.USD), Money.Of(12.50m, Currency.USD));
        Assert.True(Money.Of(1m, Currency.USD) == Money.FromMinor(100, Currency.USD));
        Assert.True(Money.Of(1m, Currency.USD) != Money.Of(1m, Currency.EUR));
    }

    [Fact]
    public void Works_as_a_dictionary_or_set_key()
    {
        var set = new HashSet<Money> { Money.Of(1m, Currency.USD), Money.Of(1.00m, Currency.USD) };
        Assert.Single(set);
    }

    [Fact]
    public void Adds_and_subtracts_same_currency()
    {
        var ten = Money.Of(10m, Currency.USD);

        Assert.Equal(Money.Of(12.5m, Currency.USD), ten + Money.Of(2.5m, Currency.USD));
        Assert.Equal(Money.Of(-5m, Currency.USD), Money.Of(5m, Currency.USD) - ten);
    }

    [Fact]
    public void Mixing_currencies_throws_with_a_clear_message()
    {
        var ex = Assert.Throws<CurrencyMismatchException>(() =>
            Money.Of(1m, Currency.USD) + Money.Of(1m, Currency.EUR));

        Assert.Contains("USD", ex.Message);
        Assert.Contains("EUR", ex.Message);
        Assert.IsAssignableFrom<InvalidOperationException>(ex);
    }

    [Fact]
    public void Compares_same_currency_only()
    {
        Assert.True(Money.Of(5m, Currency.USD) < Money.Of(10m, Currency.USD));
        Assert.True(Money.Of(10m, Currency.USD) > Money.Of(5m, Currency.USD));
        Assert.Throws<CurrencyMismatchException>(() => Money.Of(5m, Currency.USD) < Money.Of(1m, Currency.EUR));
    }

    [Theory]
    [InlineData(12.5, Currency.USD, "USD 12.50")]
    [InlineData(500, Currency.JPY, "JPY 500")]
    [InlineData(1.25, Currency.KWD, "KWD 1.250")]
    [InlineData(-5, Currency.USD, "USD -5.00")]
    public void Prints_with_the_right_decimals(double amount, Currency currency, string expected) =>
        Assert.Equal(expected, Money.Of((decimal)amount, currency).ToString());
}
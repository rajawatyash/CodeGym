using Gym.Day02;

namespace Gym.Tests.Day02;

public class MoneyFormatTests
{
    [Theory]
    [InlineData(0L, "USD", "$0.00")]
    [InlineData(5L, "USD", "$0.05")]
    [InlineData(100L, "USD", "$1.00")]
    [InlineData(99999L, "USD", "$999.99")]
    [InlineData(123456L, "USD", "$1,234.56")]
    [InlineData(123456789L, "USD", "$1,234,567.89")]
    [InlineData(-500L, "USD", "-$5.00")]
    [InlineData(100000L, "INR", "₹1,000.00")]
    [InlineData(12345678L, "INR", "₹1,23,456.78")]
    [InlineData(123456789L, "INR", "₹12,34,567.89")]
    [InlineData(10000000000L, "INR", "₹10,00,00,000.00")]
    [InlineData(-123456L, "INR", "-₹1,234.56")]
    public void Formats_amounts(long minorUnits, string currency, string expected) =>
        Assert.Equal(expected, MoneyFormat.Format(minorUnits, currency));

    [Fact]
    public void Unknown_currency_throws() =>
        Assert.Throws<ArgumentException>(() => MoneyFormat.Format(100L, "GBP"));

    [Fact]
    public void Null_currency_throws() =>
        Assert.Throws<ArgumentNullException>(() => MoneyFormat.Format(100L, null!));
}
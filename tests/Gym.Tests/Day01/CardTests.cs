using Gym.Day01;

namespace Gym.Tests.Day01;

public class CardTests
{
    [Theory]
    [InlineData("4242 4242 4242 4242")]
    [InlineData("4000 0566 5566 5556")]
    [InlineData("5555555555554444")]
    [InlineData("378282246310005")]
    public void Valid_numbers_pass(string number) =>
        Assert.True(Cards.IsValidCardNumber(number));

    [Theory]
    [InlineData("4242424242424241")]
    [InlineData("4111111111111112")]
    [InlineData("4242-4242-4242-4242")]
    [InlineData("4242")]
    [InlineData("")]
    [InlineData("abcd efgh ijkl mnop")]
    [InlineData(null)]
    public void Invalid_numbers_fail(string? number) =>
        Assert.False(Cards.IsValidCardNumber(number));

    [Theory]
    [InlineData("4242 4242 4242 4242", "Visa")]
    [InlineData("5555555555554444", "Mastercard")]
    [InlineData("2223003122003222", "Mastercard")]
    [InlineData("378282246310005", "Amex")]
    [InlineData("6011111111111117", "Unknown")]
    public void Detects_brand(string number, string expected) =>
        Assert.Equal(expected, Cards.DetectBrand(number).ToString());
}
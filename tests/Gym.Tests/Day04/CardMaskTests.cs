using Gym.Day04;

namespace Gym.Tests.Day04;

public class CardMaskTests
{
    [Theory]
    [InlineData("4242 4242 4242 4242", "**** **** **** 4242")]
    [InlineData("378282246310005", "***********0005")]
    [InlineData("1234", "1234")]
    [InlineData("12 34 5", "*2 34 5")]
    public void Keeps_only_the_last_four_digits(string number, string expected) =>
        Assert.Equal(expected, CardMask.Mask(number));

    [Theory]
    [InlineData("123")]
    [InlineData("4242-4242")]
    [InlineData("")]
    public void Bad_input_throws(string number) =>
        Assert.Throws<ArgumentException>(() => CardMask.Mask(number));

    [Fact]
    public void Null_throws() =>
        Assert.Throws<ArgumentNullException>(() => CardMask.Mask(null!));
}
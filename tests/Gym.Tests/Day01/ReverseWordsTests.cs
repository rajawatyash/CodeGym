using Gym.Day01;

namespace Gym.Tests.Day01;

public class ReverseWordsTests
{
    [Theory]
    [InlineData("hello world", "world hello")]
    [InlineData("  hello   world  ", "world hello")]
    [InlineData("pay the invoice now", "now invoice the pay")]
    [InlineData("one", "one")]
    [InlineData("", "")]
    [InlineData("   ", "")]
    public void Reverses_word_order(string input, string expected) =>
        Assert.Equal(expected, Strings.ReverseWords(input));

    [Fact]
    public void Null_throws() =>
        Assert.Throws<ArgumentNullException>(() => Strings.ReverseWords(null!));
}
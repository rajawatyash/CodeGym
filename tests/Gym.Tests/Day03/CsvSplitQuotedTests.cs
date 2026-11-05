using Gym.Day03;

namespace Gym.Tests.Day03;

public class CsvSplitQuotedTests
{
    [Theory]
    [InlineData("a,b", "a|b")]
    [InlineData("\"Smith, John\",42", "Smith, John|42")]
    [InlineData("\"say \"\"hi\"\"\",x", "say \"hi\"|x")]
    [InlineData("a,\"b\"", "a|b")]
    [InlineData("\"\"", "")]
    [InlineData(",", "|")]
    [InlineData(" a , b ", " a | b ")]
    [InlineData("ab\"c,d", "ab\"c|d")]
    public void Splits_with_quotes(string line, string expected) =>
        Assert.Equal(expected.Split('|'), CsvLine.SplitQuoted(line).ToArray());

    [Theory]
    [InlineData("\"abc")]
    [InlineData("\"a\"b,c")]
    public void Bad_quotes_throw(string line) =>
        Assert.Throws<FormatException>(() => CsvLine.SplitQuoted(line));

    [Fact]
    public void Null_throws() =>
        Assert.Throws<ArgumentNullException>(() => CsvLine.SplitQuoted(null!));
}
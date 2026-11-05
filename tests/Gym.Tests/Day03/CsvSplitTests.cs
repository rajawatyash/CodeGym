using Gym.Day03;

namespace Gym.Tests.Day03;

public class CsvSplitTests
{
    [Theory]
    [InlineData("2026-10-01, P1 ,125.50", "2026-10-01|P1|125.50")]
    [InlineData("a,,b", "a||b")]
    [InlineData("a,", "a|")]
    [InlineData(",", "|")]
    [InlineData("single", "single")]
    [InlineData("", "")]
    public void Splits_on_commas_and_trims(string line, string expected) =>
        Assert.Equal(expected.Split('|'), CsvLine.Split(line).ToArray());

    [Fact]
    public void Null_throws() =>
        Assert.Throws<ArgumentNullException>(() => CsvLine.Split(null!));
}
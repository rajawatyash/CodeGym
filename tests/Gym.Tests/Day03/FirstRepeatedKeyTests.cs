using Gym.Day03;

namespace Gym.Tests.Day03;

public class FirstRepeatedKeyTests
{
    [Theory]
    [InlineData("k1,k2,k3,k2,k1", "k2")]
    [InlineData("x,x", "x")]
    [InlineData("a,b,c", null)]
    [InlineData("a,A", null)]
    [InlineData("", null)]
    public void Finds_the_earliest_repeat(string keys, string? expected) =>
        Assert.Equal(expected, Idempotency.FirstRepeatedKey(keys.Split(',', StringSplitOptions.RemoveEmptyEntries)));

    [Fact]
    public void Null_throws() =>
        Assert.Throws<ArgumentNullException>(() => Idempotency.FirstRepeatedKey(null!));
}
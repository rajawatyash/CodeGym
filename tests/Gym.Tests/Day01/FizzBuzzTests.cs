using Gym.Day01;

namespace Gym.Tests.Day01;

public class FizzBuzzTests
{
    [Fact]
    public void Returns_items_from_1_to_n()
    {
        var result = Warmup.FizzBuzz(15);

        Assert.Equal(15, result.Count);
        Assert.Equal("1", result[0]);
        Assert.Equal("Fizz", result[2]);
        Assert.Equal("Buzz", result[4]);
        Assert.Equal("FizzBuzz", result[14]);
    }

    [Fact]
    public void Zero_gives_an_empty_list() => Assert.Empty(Warmup.FizzBuzz(0));

    [Fact]
    public void Negative_n_throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Warmup.FizzBuzz(-1));
}
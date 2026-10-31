using Gym.Day01;

namespace Gym.Tests.Day01;

public class TopWordsTests
{
    [Fact]
    public void Most_frequent_first_then_alphabetical()
    {
        var result = Strings.TopWords("the cat and the hat. The end!", 2);

        Assert.Equal(new[] { ("the", 3), ("and", 1) }, result.ToArray());
    }

    [Fact]
    public void Ties_are_alphabetical()
    {
        var result = Strings.TopWords("b a b a c", 3);

        Assert.Equal(new[] { ("a", 2), ("b", 2), ("c", 1) }, result.ToArray());
    }

    [Fact]
    public void Ignores_case_and_punctuation()
    {
        var result = Strings.TopWords("Hello, HELLO hello!", 5);

        Assert.Equal(new[] { ("hello", 3) }, result.ToArray());
    }

    [Fact]
    public void Empty_text_gives_empty_result() =>
        Assert.Empty(Strings.TopWords("", 3));

    [Fact]
    public void Zero_n_throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Strings.TopWords("a", 0));
}
using Gym.Day03;

namespace Gym.Tests.Day03;

public class StatementParserTests
{
    [Fact]
    public void Keeps_good_lines_and_reports_bad_ones()
    {
        var lines = new[]
        {
            "date,reference,amount",
            "2026-10-01,P1,125.50",
            "",
            "2026-10-02,P2",
            "2026-13-01,P3,10",
            "2026-10-03,,10",
            "2026-10-04,P5,abc",
            "2026-10-05,P6,-20",
        };

        var result = StatementParser.Parse(lines);

        Assert.Equal(
            new[]
            {
                new BankData(new DateOnly(2026, 10, 1), "P1", 125.50m),
                new BankData(new DateOnly(2026, 10, 5), "P6", -20m),
            },
            result.Lines.ToArray());
        Assert.Equal(
            new[]
            {
                "Line 4: expected 3 fields, got 2",
                "Line 5: invalid date '2026-13-01'",
                "Line 6: missing reference",
                "Line 7: invalid amount 'abc'",
            },
            result.Errors.ToArray());
    }

    [Fact]
    public void Trims_spaces_around_fields()
    {
        var result = StatementParser.Parse(new[] { "date,reference,amount", " 2026-10-01 , P1 , 5 " });

        Assert.Equal(new[] { new BankData(new DateOnly(2026, 10, 1), "P1", 5m) }, result.Lines.ToArray());
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Header_only_gives_empty_result()
    {
        var result = StatementParser.Parse(new[] { "date,reference,amount" });

        Assert.Empty(result.Lines);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void No_lines_gives_empty_result()
    {
        var result = StatementParser.Parse(Array.Empty<string>());

        Assert.Empty(result.Lines);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Null_throws() =>
        Assert.Throws<ArgumentNullException>(() => StatementParser.Parse(null!));
}
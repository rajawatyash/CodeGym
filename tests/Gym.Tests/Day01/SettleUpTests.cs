using Gym.Day01;

namespace Gym.Tests.Day01;

public class SettleUpTests
{
    private static List<BillSplitter.Transfer> Sorted(IEnumerable<BillSplitter.Transfer> transfers) =>
        transfers.OrderBy(t => t.From).ThenBy(t => t.To).ToList();

    [Fact]
    public void One_payer_split_three_ways()
    {
        var expenses = new[] { new BillSplitter.Expense("Asha", 90m, new[] { "Asha", "Ben", "Chen" }) };

        var result = BillSplitter.SettleUp(expenses);

        Assert.Equal(
            new List<BillSplitter.Transfer> { new("Ben", "Asha", 30m), new("Chen", "Asha", 30m) },
            Sorted(result));
    }

    [Fact]
    public void People_who_are_even_do_not_appear()
    {
        var expenses = new[]
        {
            new BillSplitter.Expense("Asha", 60m, new[] { "Asha", "Ben", "Chen" }),
            new BillSplitter.Expense("Ben", 30m, new[] { "Asha", "Ben", "Chen" }),
        };

        var result = BillSplitter.SettleUp(expenses);

        Assert.Equal(new List<BillSplitter.Transfer> { new("Chen", "Asha", 30m) }, Sorted(result));
    }

    [Fact]
    public void Leftover_cents_go_to_the_first_people_in_the_list()
    {
        var expenses = new[] { new BillSplitter.Expense("Ben", 10m, new[] { "Asha", "Ben", "Chen" }) };

        var result = BillSplitter.SettleUp(expenses);

        Assert.Equal(
            new List<BillSplitter.Transfer> { new("Asha", "Ben", 3.34m), new("Chen", "Ben", 3.33m) },
            Sorted(result));
    }

    [Fact]
    public void Separate_groups_settle_separately()
    {
        var expenses = new[]
        {
            new BillSplitter.Expense("Asha", 100m, new[] { "Ben" }),
            new BillSplitter.Expense("Chen", 50m, new[] { "Dev" }),
        };

        var result = BillSplitter.SettleUp(expenses);

        Assert.Equal(
            new List<BillSplitter.Transfer> { new("Ben", "Asha", 100m), new("Dev", "Chen", 50m) },
            Sorted(result));
    }

    [Fact]
    public void Everyone_even_means_no_transfers()
    {
        var expenses = new[]
        {
            new BillSplitter.Expense("Asha", 20m, new[] { "Asha", "Ben" }),
            new BillSplitter.Expense("Ben", 20m, new[] { "Asha", "Ben" }),
        };

        Assert.Empty(BillSplitter.SettleUp(expenses));
    }

    [Fact]
    public void Zero_amount_throws() =>
        Assert.Throws<ArgumentException>(() =>
            BillSplitter.SettleUp(new[] { new BillSplitter.Expense("Asha", 0m, new[] { "Ben" }) }));

    [Fact]
    public void Empty_split_throws() =>
        Assert.Throws<ArgumentException>(() =>
            BillSplitter.SettleUp(new[] { new BillSplitter.Expense("Asha", 10m, Array.Empty<string>()) }));
}
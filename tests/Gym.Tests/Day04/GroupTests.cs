using Gym.Day04;
namespace Gym.Tests.Day04;

public class GroupTests
{

    private static Group TripWith(params string[] members)
    {
        var group = new Group("Goa trip", new GreedySettlement());
        foreach (var m in members) group.AddMember(m);
        return group;
    }

    [Fact]
    public void Given_Group_Empty()
    {
        // Arrange
        var group = new Group("Goa Group", new GreedySettlement());

        // Assert
        Assert.Empty(group.Members);
    }
    [Fact]
    public void Added_member_appears_with_zero_balance()
    {
        // Arrange
        var group = new Group("Goa Group", new GreedySettlement());
        
        // Act
        group.AddMember("Asha");

        // Assert
        Assert.Contains("Asha", group.Members);
        Assert.Equal(0m, group.Balances()["Asha"]);
    }

    [Fact]
    public void One_payer_split_three_ways()
    {
        // Arrange
        var group = new Group("Goa Group", new GreedySettlement());
        group.AddMember("Asha");
        group.AddMember("Ben");
        group.AddMember("Chen");
        
        // Act
        group.AddExpense(new Expense("Asha", 90m, new[] { "Asha", "Ben", "Chen" }));
        // Assert
        var balances = group.Balances();
        Assert.Equal(60m, balances["Asha"]);
        Assert.Equal(-30m, balances["Ben"]);
        Assert.Equal(-30m, balances["Chen"]);
    }

    [Fact]
    public void Given_Group_Show_Member_Balance_Not_In_ExpenseSplit()
    {
        // Arrange
        var group = new Group("Goa Group", new GreedySettlement());
        group.AddMember("Asha");
        group.AddMember("Ben");
        group.AddMember("Chen");
        group.AddMember("Dev");
        
        // Act
        group.AddExpense(new Expense("Asha", 90m, new[] { "Asha", "Ben", "Chen" }));

        // Assert
        Assert.Contains("Dev", group.Members);
        Assert.Equal(0m, group.Balances()["Dev"]);
    }

    [Fact]
    public void One_payer_split_three_ways_SettleUp()
    {
        // Arrange
        var group = new Group("Goa Group", new GreedySettlement());
        group.AddMember("Asha");
        group.AddMember("Ben");
        group.AddMember("Chen");
        group.AddExpense(new Expense("Asha", 90m, new[] { "Asha", "Ben", "Chen" }));
        
        // Act
        var result = group.SettleUp();

        // Assert
        Assert.Equal(
            new List<Transfer> { new("Ben", "Asha", 30m), new("Chen", "Asha", 30m) },
            result);
    }

    [Fact]
    public void Leftover_cents_go_to_the_first_people_in_the_list()
    {
        // Arrange
        var group = new Group("Goa Group", new GreedySettlement());
        group.AddMember("Asha");
        group.AddMember("Ben");
        group.AddMember("Chen");
        var expenses = new Expense("Ben", 10m, new[] { "Asha", "Ben", "Chen" });
        group.AddExpense(expenses);

        var result = group.SettleUp();

        Assert.Equal(
            new List<Transfer> { new("Asha", "Ben", 3.34m), new("Chen", "Ben", 3.33m) },
            result);
    }

    [Fact]
    public void Blank_GroupName_throw()
    {

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Group(" ", new GreedySettlement()));
    }

    [Fact]
    public void Null_Strategy_throw()
    {

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new Group("Goa Group", null));
    }

    [Fact]
    public void Blank_WhiteSpace_Name_throw()
    {
        var group = new Group("Goa Group", new GreedySettlement());
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            group.AddMember(" "));
    }

    [Fact]
    public void Adding_same_member_twice_throws()
    {
        // Arrange
        var group = new Group("Goa Group", new GreedySettlement());
        group.AddMember("Asha");
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            group.AddMember("Asha"));
    }

    // 12. Null expense
    [Fact]
    public void Null_expense_throws()
    {
        var group = TripWith("Asha", "Ben");
        Assert.Throws<ArgumentNullException>(() => group.AddExpense(null!));
    }

    // 13. Zero or negative amount: one test, two inputs
    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Zero_or_negative_amount_throws(int amount)
    {
        var group = TripWith("Asha", "Ben");
        Assert.Throws<ArgumentException>(() =>
            group.AddExpense(new Expense("Asha", amount, new[] { "Asha", "Ben" })));
    }

    // 14. Empty split list
    [Fact]
    public void Empty_split_list_throws()
    {
        var group = TripWith("Asha", "Ben");
        Assert.Throws<ArgumentException>(() =>
            group.AddExpense(new Expense("Asha", 30m, Array.Empty<string>())));
    }

    // 15. Someone in the split isn't a member
    [Fact]
    public void Stranger_in_split_throws()
    {
        var group = TripWith("Asha", "Ben");
        Assert.Throws<InvalidOperationException>(() =>
            group.AddExpense(new Expense("Asha", 30m, new[] { "Ben", "Zoe" })));
    }

    // 16. The payer isn't a member
    [Fact]
    public void Stranger_as_payer_throws()
    {
        var group = TripWith("Asha", "Ben");
        Assert.Throws<InvalidOperationException>(() =>
            group.AddExpense(new Expense("Zoe", 30m, new[] { "Asha", "Ben" })));
    }

    // 17. All or nothing: a rejected expense changes nothing
    [Fact]
    public void Rejected_expense_leaves_balances_unchanged()
    {
        var group = TripWith("Asha", "Ben");
        group.AddExpense(new Expense("Asha", 20m, new[] { "Asha", "Ben" }));   // Asha +10, Ben -10

        Assert.Throws<InvalidOperationException>(() =>
            group.AddExpense(new Expense("Asha", 30m, new[] { "Ben", "Zoe" })));

        var balances = group.Balances();
        Assert.Equal(10m, balances["Asha"]);    // still the same as before
        Assert.Equal(-10m, balances["Ben"]);    // Ben was NOT charged for the rejected one
    }

    // 18. SettleUp hands the balances, in cents, to the strategy
    [Fact]
    public void Strategy_receives_balances_in_cents()
    {
        var spy = new RecordingStrategy();
        var group = new Group("Goa trip", spy);
        group.AddMember("Asha");
        group.AddMember("Ben");
        group.AddExpense(new Expense("Asha", 20m, new[] { "Asha", "Ben" }));

        group.SettleUp();

        Assert.NotNull(spy.Received);
        Assert.Equal(1000L, spy.Received!["Asha"]);    // +10.00 in cents
        Assert.Equal(-1000L, spy.Received["Ben"]);
    }

    // A fake strategy that only records what it was given (put it inside GroupTests too)
    private sealed class RecordingStrategy : ISettlementStrategy
    {
        public IReadOnlyDictionary<string, long>? Received { get; private set; }

        public IReadOnlyList<Transfer> Settle(IReadOnlyDictionary<string, long> balancesInCents)
        {
            Received = balancesInCents;
            return Array.Empty<Transfer>();
        }
    }
    
}
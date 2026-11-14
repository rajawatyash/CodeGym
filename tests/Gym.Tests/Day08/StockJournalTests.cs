using Gym.Day08.Twin;

namespace Gym.Tests.Day08.Twin;

public class StockJournalTests
{
    private static StockLine In(string place, int qty) => new(place, Direction.In, qty);
    private static StockLine Out(string place, int qty) => new(place, Direction.Out, qty);

    // Built fresh each call, so each call makes NEW arrays (that's what tests the record trap).
    private static StockTransfer Delivery() => new("T0", "Delivery", new[] { Out("supplier", 50), In("warehouse", 50) });
    private static StockTransfer Restock(string note = "Restock shop") => new("T1", note, new[] { Out("warehouse", 10), In("shop", 10) });

    [Fact]
    public void Recording_moves_stock()
    {
        var journal = new StockJournal(new InMemoryTransferStore());

        Assert.True(journal.Record(Delivery()));
        Assert.True(journal.Record(Restock()));

        Assert.Equal(40, journal.Stock("warehouse"));
        Assert.Equal(10, journal.Stock("shop"));
        Assert.Equal(-50, journal.Stock("supplier"));   // the outside world; all places add up to 0
        Assert.Equal(0, journal.Stock("never_used"));
    }

    [Fact]
    public void Same_transfer_twice_is_ignored()
    {
        var journal = new StockJournal(new InMemoryTransferStore());
        journal.Record(Restock());

        Assert.False(journal.Record(Restock()));       // a new object with equal content
        Assert.Equal(10, journal.Stock("shop"));
    }

    [Fact]
    public void Same_id_with_different_content_throws()
    {
        var journal = new StockJournal(new InMemoryTransferStore());
        journal.Record(Restock());

        Assert.Throws<InvalidOperationException>(() => journal.Record(Restock("Something else")));
    }

    [Fact]
    public void Unbalanced_transfer_is_rejected_and_nothing_changes()
    {
        var journal = new StockJournal(new InMemoryTransferStore());

        Assert.Throws<ArgumentException>(() =>
            journal.Record(new StockTransfer("T9", "Oops", new[] { Out("a", 10), In("b", 9) })));
        Assert.Equal(0, journal.Stock("a"));
    }

    [Fact]
    public void State_lives_in_the_store()
    {
        var store = new InMemoryTransferStore();
        new StockJournal(store).Record(Restock());

        var journalTwo = new StockJournal(store);      // a second journal on the same store

        Assert.Equal(10, journalTwo.Stock("shop"));
        Assert.False(journalTwo.Record(Restock()));    // it sees the first one's data
    }
}
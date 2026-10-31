using Gym.Day02;

namespace Gym.Tests.Day02;

public class ReconcilerTests
{
    [Fact]
    public void Sorts_every_entry_into_one_bucket()
    {
        var ledger = new[]
        {
            new LedgerEntry("P1", 10m), new LedgerEntry("P2", 20m),
            new LedgerEntry("P3", 30m), new LedgerEntry("P4", 5m),
        };
        var bank = new[]
        {
            new BankLine("P2", 20m), new BankLine("P3", 35m),
            new BankLine("P5", 7m), new BankLine("P1", 10m),
        };

        var result = Reconciler.Reconcile(ledger, bank);

        Assert.Equal(new[] { "P1", "P2" }, result.Matched.ToArray());
        Assert.Equal(new[] { new Mismatch("P3", 30m, 35m) }, result.Mismatched.ToArray());
        Assert.Equal(new[] { "P4" }, result.MissingInBank.ToArray());
        Assert.Equal(new[] { "P5" }, result.MissingInLedger.ToArray());
    }

    [Fact]
    public void Ignores_spaces_around_references()
    {
        var result = Reconciler.Reconcile(
            new[] { new LedgerEntry(" P1 ", 10m) },
            new[] { new BankLine("P1", 10m) });

        Assert.Equal(new[] { "P1" }, result.Matched.ToArray());
    }

    [Fact]
    public void References_are_case_sensitive()
    {
        var result = Reconciler.Reconcile(
            new[] { new LedgerEntry("p1", 10m) },
            new[] { new BankLine("P1", 10m) });

        Assert.Empty(result.Matched);
        Assert.Equal(new[] { "p1" }, result.MissingInBank.ToArray());
        Assert.Equal(new[] { "P1" }, result.MissingInLedger.ToArray());
    }

    [Fact]
    public void Each_list_is_sorted_by_reference()
    {
        // Both inputs are deliberately in reverse order, so every list must be sorted.
        var ledger = new[]
        {
            new LedgerEntry("Z9", 1m), new LedgerEntry("Y8", 3m), new LedgerEntry("X1", 4m),
            new LedgerEntry("M5", 1m), new LedgerEntry("B2", 5m), new LedgerEntry("A1", 1m),
        };
        var bank = new[]
        {
            new BankLine("Q7", 1m), new BankLine("B2", 6m), new BankLine("C3", 2m),
            new BankLine("X1", 9m), new BankLine("M5", 1m), new BankLine("Y8", 3m),
        };

        var result = Reconciler.Reconcile(ledger, bank);

        Assert.Equal(new[] { "M5", "Y8" }, result.Matched.ToArray());
        Assert.Equal(
            new[] { new Mismatch("B2", 5m, 6m), new Mismatch("X1", 4m, 9m) },
            result.Mismatched.ToArray());
        Assert.Equal(new[] { "A1", "Z9" }, result.MissingInBank.ToArray());
        Assert.Equal(new[] { "C3", "Q7" }, result.MissingInLedger.ToArray());
    }

    [Fact]
    public void Empty_lists_give_empty_result()
    {
        var result = Reconciler.Reconcile(Array.Empty<LedgerEntry>(), Array.Empty<BankLine>());

        Assert.Empty(result.Matched);
        Assert.Empty(result.Mismatched);
        Assert.Empty(result.MissingInBank);
        Assert.Empty(result.MissingInLedger);
    }

    [Fact]
    public void Duplicate_in_ledger_throws() =>
        Assert.Throws<ArgumentException>(() => Reconciler.Reconcile(
            new[] { new LedgerEntry("P1", 1m), new LedgerEntry("P1 ", 2m) },
            Array.Empty<BankLine>()));

    [Fact]
    public void Duplicate_in_bank_throws() =>
        Assert.Throws<ArgumentException>(() => Reconciler.Reconcile(
            Array.Empty<LedgerEntry>(),
            new[] { new BankLine("P1", 1m), new BankLine("P1", 1m) }));

    [Fact]
    public void Null_list_throws() =>
        Assert.Throws<ArgumentNullException>(() => Reconciler.Reconcile(null!, Array.Empty<BankLine>()));
}
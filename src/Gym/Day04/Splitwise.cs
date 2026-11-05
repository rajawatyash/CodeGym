namespace Gym.Day04;

public record Expense(string PaidBy, decimal Amount, IReadOnlyList<string> SplitBetween);
public record Transfer(string From, string To, decimal Amount);

public interface ISettlementStrategy
{
    IReadOnlyList<Transfer> Settle(IReadOnlyDictionary<string, long> balancesInCents);
}

public sealed class GreedySettlement : ISettlementStrategy
{
    public IReadOnlyList<Transfer> Settle(IReadOnlyDictionary<string, long> balancesInCents)
    {
        var balances = new Dictionary<string, long>(balancesInCents);
        var transfers = new List<Transfer>();

        while (true)
        {
            // Biggest debtor and biggest creditor each round; ties go to the name that sorts first.
            var debtor = balances
                .Where(b => b.Value < 0)
                .OrderBy(b => b.Value)
                .ThenBy(b => b.Key, StringComparer.Ordinal)
                .FirstOrDefault();

            var creditor = balances
                .Where(b => b.Value > 0)
                .OrderByDescending(b => b.Value)
                .ThenBy(b => b.Key, StringComparer.Ordinal)
                .FirstOrDefault();

            if (debtor.Key == null || creditor.Key == null)
                break;

            long cents = Math.Min(-debtor.Value, creditor.Value);
            transfers.Add(new Transfer(debtor.Key, creditor.Key, cents / 100m));

            balances[debtor.Key] += cents;
            balances[creditor.Key] -= cents;
        }

        return transfers;
    }
}

public sealed class Group
{
    private readonly Dictionary<string, long> _balances = new();
    private readonly ISettlementStrategy _strategy;

    public Group(string name, ISettlementStrategy strategy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Group name is required.", nameof(name));
        ArgumentNullException.ThrowIfNull(strategy);

        Name = name;
        _strategy = strategy;
    }

    public string Name { get; }

    public IReadOnlyCollection<string> Members => _balances.Keys;

    public void AddMember(string memberName)
    {
        if (string.IsNullOrWhiteSpace(memberName))
            throw new ArgumentException("Member Name is required", nameof(memberName));

        if (_balances.ContainsKey(memberName))
            throw new InvalidOperationException($"'{memberName}' is already a member.");

        _balances[memberName] = 0;
    }
    public void AddExpense(Expense expense)
    {
        // ---------- Part A: check everything, change nothing ----------
        ArgumentNullException.ThrowIfNull(expense);

        if (expense.Amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(expense));

        if (expense.SplitBetween is null || expense.SplitBetween.Count == 0)
            throw new ArgumentException("Split list must not be empty.", nameof(expense));

        if (!_balances.ContainsKey(expense.PaidBy))
            throw new InvalidOperationException($"Payer '{expense.PaidBy}' is not a member.");

        foreach (var person in expense.SplitBetween)
            if (!_balances.ContainsKey(person))
                throw new InvalidOperationException($"'{person}' is not a member.");

        // ---------- Part B: all checks passed, now change state ----------
        long totalCents = (long)(expense.Amount * 100);   // 90.00m -> 9000
        int people = expense.SplitBetween.Count;
        long share = totalCents / people;                 // 1000 / 3 = 333 (whole-number division, which is what we WANT here)
        long leftover = totalCents % people;              // 1000 % 3 = 1 cent left over

        _balances[expense.PaidBy] += totalCents;          // A: the payer put in the whole amount

        for (int i = 0; i < people; i++)
        {
            long owes = share + (i < leftover ? 1 : 0);   // the first 'leftover' people pay 1 extra cent
            _balances[expense.SplitBetween[i]] -= owes;   // B: each person used their share
        }
    }
    public IReadOnlyDictionary<string, decimal> Balances()
    {
        var balanceDict = new Dictionary<string, decimal>();
        foreach (var balance in _balances)
        {
            var decimalAmount = balance.Value / 100m;

            balanceDict[balance.Key] = decimalAmount;
        }
        return balanceDict;
    }
    public IReadOnlyList<Transfer> SettleUp() => _strategy.Settle(_balances);
}
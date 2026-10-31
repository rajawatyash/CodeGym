using System.Text;

namespace Gym.Day01
{
    public static class BillSplitter
    {
        public record Expense(string PaidBy, decimal Amount, IReadOnlyList<string> SplitBetween);
        public record Transfer(string From, string To, decimal Amount);

        public static IReadOnlyList<Transfer> SettleUp(IEnumerable<Expense> expenses)
        {
            if (expenses == null)
                throw new ArgumentNullException(nameof(expenses));

            // Balances in whole cents: paid minus owed. Positive = should receive money.
            var balances = new Dictionary<string, long>();

            foreach (var expense in expenses)
            {
                if (expense.Amount <= 0)
                    throw new ArgumentException("Expense amount must be positive.", nameof(expenses));
                if (expense.SplitBetween == null || expense.SplitBetween.Count == 0)
                    throw new ArgumentException("SplitBetween must contain at least one person.", nameof(expenses));

                long totalCents = (long)Math.Round(expense.Amount * 100, MidpointRounding.AwayFromZero);
                int count = expense.SplitBetween.Count;
                long shareCents = totalCents / count;   // each share rounded down
                long leftover = totalCents % count;     // extra cents go to the first people in list order

                balances[expense.PaidBy] = balances.GetValueOrDefault(expense.PaidBy) + totalCents;

                for (int i = 0; i < count; i++)
                {
                    string person = expense.SplitBetween[i];
                    long owed = shareCents + (i < leftover ? 1 : 0);
                    balances[person] = balances.GetValueOrDefault(person) - owed;
                }
            }

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
}
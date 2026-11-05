using System.Security.Cryptography.X509Certificates;

namespace Gym.Day03
{
    public record LedgerEntry(string Reference, decimal Amount);
    public record BankLine(string Reference, decimal Amount);
    public record Mismatch(string Reference, decimal LedgerAmount, decimal BankLineAount);
    public record ReconcileResult(
        IReadOnlyList<string> Matched,
        IReadOnlyList<Mismatch> Mismatched,
        IReadOnlyList<string> MissingInBank,
        IReadOnlyList<string> MissingInLedger
    );
    public static class Reconciler
    {
        public static ReconcileResult Reconcile(IReadOnlyList<LedgerEntry> ledger, IReadOnlyList<BankLine> bank)
        {
            ArgumentNullException.ThrowIfNull(ledger);
            ArgumentNullException.ThrowIfNull(bank);

            var bankDataList = new Dictionary<string, decimal>();
            
            foreach(var data in bank)
            {
                if(!bankDataList.TryAdd(data.Reference.Trim(), data.Amount))
                {
                    throw new ArgumentException($"Duplicate bank Reference : {data.Reference}");
                }
            }

            var matched = new List<string>();
            var mismatched = new List<Mismatch>();
            var missingInBank = new List<string>();
            var presentInLedger = new HashSet<string>();

            foreach( var entry in ledger)
            {
                string reference = entry.Reference.Trim();
                if(!presentInLedger.Add(reference))
                {
                    throw new ArgumentException($"Duplicate ledger Reference : {entry.Reference}");
                }
                if(!bankDataList.Remove(reference, out decimal bankAmount))
                {
                    missingInBank.Add(reference);
                }
                else if (bankAmount == entry.Amount)
                {
                    matched.Add(reference);
                }
                else
                {
                    mismatched.Add(new Mismatch(reference, entry.Amount, bankAmount));
                }
            }

            var misssingInLedger = new List<string>(bankDataList.Keys);

            matched.Sort(string.CompareOrdinal);
            mismatched.Sort((x,y) => string.CompareOrdinal(x.Reference, y.Reference));
            missingInBank.Sort(string.CompareOrdinal);
            misssingInLedger.Sort(string.CompareOrdinal);

            return new ReconcileResult(matched, mismatched, missingInBank, misssingInLedger);

        }
    }
}
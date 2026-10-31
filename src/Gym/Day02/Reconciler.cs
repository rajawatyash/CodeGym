namespace Gym.Day02
{
    public record LedgerEntry(string Reference, decimal Amount);
    public record BankLine(string Reference, decimal Amount);
    public record Mismatch(string Reference, decimal LedgerAmount, decimal BankAmount);
    public record ReconcileResult(
        IReadOnlyList<string> Matched,
        IReadOnlyList<Mismatch> Mismatched,
        IReadOnlyList<string> MissingInBank,
        IReadOnlyList<string> MissingInLedger);

    public static class Reconciler
    {
        public static ReconcileResult Reconcile(IReadOnlyList<LedgerEntry> ledger, IReadOnlyList<BankLine> bank)
        {
            List<string> Matched = new();
            List<Mismatch> Mismatched = new();
            List<string> MissingInBank = new();
            List<string> MissingInLedger = new();
            if(ledger == null || bank == null)
            {
                throw new ArgumentNullException("Either List Null");
            }

            Dictionary<string, decimal> ledgerDict = new Dictionary<string, decimal>();
            Dictionary<string, decimal> bankDict = new Dictionary<string, decimal>();

            foreach(var ledge in ledger)
            {
                ledgerDict.Add(ledge.Reference.Trim(), ledge.Amount);
            }
            foreach(var bankData in bank)
            {
                bankDict.Add(bankData.Reference.Trim(), bankData.Amount);
            }

            foreach(var ledge in ledgerDict)
            {
                if(bankDict.TryGetValue(ledge.Key, out decimal amount))
                {
                    if(amount == ledge.Value)
                    {
                        Matched.Add(ledge.Key);
                    }
                    else
                    {
                        Mismatched.Add(new Mismatch(ledge.Key, ledge.Value, amount));
                    }
                    ledgerDict.Remove(ledge.Key);
                    bankDict.Remove(ledge.Key);
                }
            }

            if(Matched.Any())
            {
                Matched.Sort(string.CompareOrdinal);
            }
            if(Mismatched.Any())
            {
                Mismatched.Sort((x, y) => string.Compare(x.Reference, y.Reference));
            }

            if(ledgerDict.Any())
            {
                foreach(var ledge in ledgerDict)
                {
                    MissingInBank.Add(ledge.Key);
                }
                MissingInBank.Sort(string.CompareOrdinal);
                
            }

            if(bankDict.Any())
            {
                foreach(var bankData in bankDict)
                {
                    MissingInLedger.Add(bankData.Key);
                }

                MissingInLedger.Sort(string.CompareOrdinal);
                
            }

            return new ReconcileResult(Matched, Mismatched, MissingInBank, MissingInLedger);
        }
    }

    
}
namespace Gym.Day04;
public record Payout(string Id, decimal Amount);

public static class PayoutBatcher
{
    public static List<List<Payout>> Batch(IReadOnlyList<Payout> payouts, int maxCount, decimal maxTotal)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(maxCount, 1);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(maxTotal, 0m);
        ArgumentNullException.ThrowIfNull(payouts);

        decimal currentMaxTotal = 0m;

        var batches = new List<List<Payout>>();
        List<Payout> current = new List<Payout>();


        foreach(var p in payouts)
        {
            if (p.Amount <= 0m)
                throw new ArgumentException($"Payout {p.Id} must be positive.", nameof(payouts));
            if (p.Amount > maxTotal)
                throw new ArgumentException($"Payout {p.Id} is bigger than the max total.", nameof(payouts));
            if(current.Count + 1 > maxCount || currentMaxTotal + p.Amount > maxTotal)
            {
                batches.Add(current);
                current = new List<Payout>();
                currentMaxTotal = 0m;
            }
            current.Add(p);
            currentMaxTotal += p.Amount;
        }

        if (current.Count > 0)
            batches.Add(current);
        return batches;
    }
}
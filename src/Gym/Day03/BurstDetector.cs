namespace Gym.Day03
{
    public record Charge(string CardId, DateTime At, decimal Amount);

    public static class BurstDetector
    {
        public static IReadOnlyList<string> FlagBursts(IReadOnlyList<Charge> charges, int maxCharges, TimeSpan window)
        {
            ArgumentNullException.ThrowIfNull(charges);
            ArgumentOutOfRangeException.ThrowIfLessThan(maxCharges, 1);
            if (window <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(window), "Window must be positive.");

            var chargeDict = new Dictionary<string, Queue<DateTime>>();
            var flagged = new HashSet<string>();

            for( int i = 0; i < charges.Count; i++)
            {
                var charge = charges[i];
                if (i > 0 && charge.At < charges[i - 1].At)
                {
                    throw new ArgumentException("Charges must be sorted by time.", nameof(charges));
                }
                if (!chargeDict.TryGetValue(charge.CardId, out var queue))
                {
                    queue = new Queue<DateTime>();
                    chargeDict[charge.CardId] = queue;
                }
                
                while(queue.Count > 0 && charge.At - queue.Peek() >= window)
                {
                    queue.Dequeue();
                }

                queue.Enqueue(charge.At);

                if(queue.Count > maxCharges)
                {
                    flagged.Add(charge.CardId);
                }
            }

            return flagged.Order(StringComparer.Ordinal).ToList();
        }
    }
}
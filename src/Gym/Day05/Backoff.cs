namespace Gym.Day05;

public static class Backoff
{
    public static List<TimeSpan> Delays(int retries, TimeSpan baseDelay, TimeSpan maxDelay)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(retries, 0);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(baseDelay, TimeSpan.Zero);

        if(maxDelay < baseDelay) throw new ArgumentException("MaxDelay should be greater than baseDelay", nameof(maxDelay));
        List<TimeSpan> delays = [];
        var current = baseDelay;

        for (int i = 0; i < retries; i++)
        {
            delays.Add(current);                 // exactly one delay per retry

            if (current > maxDelay / 2)          // doubling would pass the cap...
                current = maxDelay;              // ...so stay at the cap from now on
            else
                current *= 2;
        }

        return delays;
    }
}
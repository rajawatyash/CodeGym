namespace Gym.Day05;

public record TrafficInput(long PaymentsPerDay, double ReadsPerWrite, int BytesPerPayment, int RetentionYears, double PeakFactor);
public record TrafficEstimate(double WritesPerSecond, double PeakWritesPerSecond, double ReadsPerSecond, double StorageGb);

public static class Capacity
{
    public static TrafficEstimate Estimate(TrafficInput input)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(input.PaymentsPerDay, 0);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(input.PeakFactor, 0);
        ArgumentOutOfRangeException.ThrowIfLessThan(input.ReadsPerWrite, 0);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(input.BytesPerPayment, 0);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(input.RetentionYears, 0);
        double writesPerSecond = (double)input.PaymentsPerDay / 86400;
        double peakWritesPerSecond = writesPerSecond * input.PeakFactor;
        double readsPerSecond = writesPerSecond * input.ReadsPerWrite;
        double storageGb = (double)input.PaymentsPerDay * input.BytesPerPayment * 365 * input.RetentionYears / 1000000000;

        return new TrafficEstimate(writesPerSecond, peakWritesPerSecond, readsPerSecond, storageGb);
    }
}
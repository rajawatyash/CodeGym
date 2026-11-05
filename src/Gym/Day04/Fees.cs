namespace Gym.Day04;

public static class Fees
{
    public static (decimal Fee, decimal Net) Calculate(
    decimal amount, bool international, decimal percent = 2.9m, decimal fixedFee = 0.30m)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(amount, 0);

        decimal fee = Math.Round(amount * (percent/100) + fixedFee, 2, MidpointRounding.AwayFromZero);

        if(international)
        {
            decimal internationFee = Math.Round(amount * 1.5m/100, 2,MidpointRounding.AwayFromZero);
            fee += internationFee;
        }

        if(fee > amount) throw new ArgumentException("Fee is more than amount", nameof(fee));

        return (fee, amount - fee);

    }
}
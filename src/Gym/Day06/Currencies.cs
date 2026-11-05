namespace Gym.Day06;


public enum Currency { USD, EUR, INR, JPY, KWD }

public static class Currencies
{
    public static int MinorDigits(Currency currency)   // JPY 0, KWD 3, the rest 2
    {
        return currency switch
        {
            Currency.USD => 2,
            Currency.EUR => 2,
            Currency.INR => 2,
            Currency.JPY => 0,
            Currency.KWD => 3,
            _ => throw new ArgumentOutOfRangeException(nameof(currency)),
        };
    }
    public static long Factor(Currency currency)       // 10 to the power MinorDigits: 1, 100 or 1000
    {
        return currency switch
        {
            Currency.USD => 100L,
            Currency.EUR => 100L,
            Currency.INR => 100L,
            Currency.JPY => 1L,
            Currency.KWD => 1000L,
            _ => throw new ArgumentOutOfRangeException(nameof(currency)),
        };
    }
}
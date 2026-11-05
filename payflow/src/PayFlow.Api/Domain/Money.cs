using System.Globalization;

namespace PayFlow.Domain;

public sealed class CurrencyMismatchException : InvalidOperationException
{
    public CurrencyMismatchException(Currency left, Currency right)
        : base($"Cannot combine {left} with {right}.") { }   // ": base(...)" passes the message to the parent class
}

public readonly record struct Money
{
    public long MinorUnits { get; }
    public Currency Currency { get; }

    private Money(long minorUnits, Currency currency)
    {
        MinorUnits = minorUnits;
        Currency = currency;
    }

    public static Money Of(decimal amount, Currency currency)
    {
        if (!TryToMinor(amount, currency, out long minor))
        throw new ArgumentException(
            $"{amount} has too many decimal places for {currency}.", nameof(amount));

        return new Money(minor, currency);
    }
    public static Money FromMinor(long minorUnits, Currency currency)
    {
        Currencies.MinorDigits(currency);       // throws for (Currency)99, so a bad currency can't get in
        return new Money(minorUnits, currency);
    }

    private static bool TryToMinor(decimal amount, Currency currency, out long minor)
    {
        minor = 0;                              // an out parameter must be set on every path

        decimal scaled = amount * Currencies.Factor(currency);   // 12.505 USD -> 1250.5
        if (scaled != decimal.Truncate(scaled))                  // something after the point?
            return false;                                        // too many decimals

        minor = (long)scaled;                                    // 1250.0 -> 1250
        return true;
    }

    public decimal Amount => MinorUnits / (decimal)Currencies.Factor(Currency);   // 1250 / 100m = 12.50;

    public static Money operator +(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return new Money(left.MinorUnits + right.MinorUnits, left.Currency);
    }

    public static Money operator -(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return new Money(left.MinorUnits - right.MinorUnits, left.Currency);
    }

    public static bool operator <(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return left.MinorUnits < right.MinorUnits;
    }

    public static bool operator >(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return left.MinorUnits > right.MinorUnits;
    }

    public override string ToString()
    {
        int digits = Currencies.MinorDigits(Currency);
        return $"{Currency} {Amount.ToString("F" + digits, CultureInfo.InvariantCulture)}";
    }

    private static void EnsureSameCurrency(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new CurrencyMismatchException(left.Currency, right.Currency);
    }

    public static bool TryParse(string? text, out Money money)
    {
        money = default;
        var parts = text?.Split(' ');
        if(parts?.Length != 2) return false;

        if(!parts[0].All(char.IsLetter)) return false;

        if (!Enum.TryParse(parts[0], ignoreCase: false, out Currency currency)) return false;

        if (!decimal.TryParse(parts[1], NumberStyles.Number, CultureInfo.InvariantCulture, out var amount)) return false;
        if (!TryToMinor(amount, currency, out var minor)) return false;
        money = new Money(minor, currency);
        return true;
        
    }
    public static Money Parse(string text)
    {
        if (!TryParse(text, out var m)) throw new FormatException($"Invalid money: '{text}'.");
        return m;
    }

    // public Money[] Allocate(params int[] ratios)
    // {
        
    // }
}
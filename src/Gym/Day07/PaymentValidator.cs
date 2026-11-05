namespace Gym.Day07;

public sealed class PaymentValidator
{
    public string? Validate(PaymentRequest r)
    {
        if (r.Amount <= 0) return "Amount must be positive"; 
        if (r.Currency != "USD" && r.Currency != "EUR" && r.Currency != "INR") return "Unsupported currency";
        if (r.CardNumber == null || r.CardNumber.Replace(" ", "").Length < 13) return "Invalid card";
        return null;
    }
}
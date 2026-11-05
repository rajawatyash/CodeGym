namespace Gym.Day07;

public record PaymentRequest(string CustomerEmail, decimal Amount, string Currency, string Provider, string CardNumber);
public record PaymentResult(bool Success, string? ChargeId, decimal Fee, string? Error);

public class PaymentProcessor
{
    public List<string> SentEmails = new();
    public List<string> Log = new();
    private readonly PaymentValidator _validator;
    private readonly Dictionary<string, IPaymentProvider> _provider;
    private readonly IEmailSender _email;

    public PaymentProcessor(PaymentValidator validator, IEnumerable<IPaymentProvider> provider, IEmailSender email)
    {
        _validator = validator;
        _provider = provider.ToDictionary(c => c.Name);
        _email = email;
    }

    public PaymentResult Process(PaymentRequest r)
    {
        // validation
        var error = _validator.Validate(r);
        if (error is not null) return new PaymentResult(false, null, 0, error);

        // fee
        if (!_provider.TryGetValue(r.Provider, out var provider))   // 2. find the carrier
            return new PaymentResult(false, null, 0, "Unknown provider");
        var fee =  provider.FeeFor(r.Amount);
        
        // charge
        var charge = provider.Charge(r.Amount, r.Currency, r.CardNumber);

        if (!charge.Success) return new PaymentResult(false, null, 0, "Declined");

        // email
        _email.Send(r);
        return new PaymentResult(true, charge.ChargeId, fee, null);
    }
}
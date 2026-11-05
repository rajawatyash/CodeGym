namespace Gym.Day07;

public record ChargeOutcome(bool Success, string? ChargeId);
public interface IPaymentProvider
{
    string Name { get; }
    decimal FeeFor(decimal amount);
    ChargeOutcome Charge(decimal amount, string currency, string card);
}

public sealed class StripeProvider : IPaymentProvider
{
    private readonly FakeStripeClient _api;
    public StripeProvider(FakeStripeClient api) => _api = api;
    public string Name => "stripe";
    public decimal FeeFor(decimal amount) => Math.Round(amount * 0.029m + 0.30m, 2, MidpointRounding.AwayFromZero);
    public ChargeOutcome Charge(decimal amount, string currency, string card)
    {
        var resp = _api.CreateCharge((long)(amount * 100), currency.ToLower(), card);
        if (resp.Status != null && resp.Status != "succeeded") return new ChargeOutcome(false, null);
        return new ChargeOutcome(true, resp.Id);
    }
}

public sealed class AdyenProvider : IPaymentProvider          // adapter around the third-party API
{
    private readonly FakeAdyenClient _api;
    public AdyenProvider(FakeAdyenClient api) => _api = api;

    public string Name => "adyen";
    public decimal FeeFor(decimal amount) => Math.Round(amount * 0.026m + 0.10m, 2, MidpointRounding.AwayFromZero);

    public ChargeOutcome Charge(decimal amount, string currency, string card)
    {
        var resp = _api.Authorise(new AdyenAmount(currency, (long)(amount * 100)), card);
        if (resp.ResultCode != null && resp.ResultCode != "Authorised") return new ChargeOutcome(false, null);
        return new ChargeOutcome(true, resp.PspReference);
    }
}
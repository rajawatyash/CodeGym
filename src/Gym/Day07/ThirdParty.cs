namespace Gym.Day07;

public record StripeCharge(string Id, string Status);

public class FakeStripeClient
{
    // Declines any card ending in 0002, like Stripe's test card 4000 0000 0000 0002.
    public StripeCharge CreateCharge(long amountInCents, string currency, string card) =>
        card.Replace(" ", "").EndsWith("0002")
            ? new StripeCharge("", "failed")
            : new StripeCharge("ch_" + amountInCents + currency, "succeeded");
}

public record AdyenAmount(string Currency, long Value);
public record AdyenResponse(string PspReference, string ResultCode);

public class FakeAdyenClient
{
    public AdyenResponse Authorise(AdyenAmount amount, string card) =>
        card.Replace(" ", "").EndsWith("0002")
            ? new AdyenResponse("", "Refused")
            : new AdyenResponse("psp_" + amount.Value + amount.Currency, "Authorised");
}
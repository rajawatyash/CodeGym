using Gym.Day07;

namespace Gym.Tests.Day07;

public class PaymentProcessorTests
{
    // The composition root: the ONLY test code you're allowed to change.
    // When the constructor starts taking dependencies, wire them up here.
    private static (PaymentProcessor Processor, IReadOnlyList<string> Emails) Create()
    {
        var emails = new InMemoryEmailSender();
        var processor = new PaymentProcessor(
            new PaymentValidator(),
            new IPaymentProvider[]
            {
                new StripeProvider(new FakeStripeClient()),
                new AdyenProvider(new FakeAdyenClient()),
            },
            emails);
        return (processor, emails.Sent);
    }

    private static PaymentRequest Request(string provider = "stripe", decimal amount = 100m,
        string currency = "USD", string card = "4242 4242 4242 4242") =>
        new("a@x.com", amount, currency, provider, card);

    [Fact]
    public void Stripe_success()
    {
        var (processor, emails) = Create();

        var result = processor.Process(Request());

        Assert.Equal(new PaymentResult(true, "ch_10000usd", 3.20m, null), result);
        Assert.Equal(new[] { "To a@x.com: you paid 100.00 USD" }, emails.ToArray());
    }

    [Fact]
    public void Adyen_success()
    {
        var (processor, emails) = Create();

        var result = processor.Process(Request(provider: "adyen"));

        Assert.Equal(new PaymentResult(true, "psp_10000USD", 2.70m, null), result);
        Assert.Single(emails);
    }

    [Theory]
    [InlineData("stripe")]
    [InlineData("adyen")]
    public void Declined_card_sends_no_email(string provider)
    {
        var (processor, emails) = Create();

        var result = processor.Process(Request(provider: provider, card: "4000 0000 0000 0002"));

        Assert.Equal(new PaymentResult(false, null, 0m, "Declined"), result);
        Assert.Empty(emails);
    }

    [Theory]
    [InlineData(0, "USD", "4242 4242 4242 4242", "stripe", "Amount must be positive")]
    [InlineData(10, "GBP", "4242 4242 4242 4242", "stripe", "Unsupported currency")]
    [InlineData(10, "USD", "4242", "stripe", "Invalid card")]
    [InlineData(10, "USD", "4242 4242 4242 4242", "paypal", "Unknown provider")]
    public void Validation_errors(int amount, string currency, string card, string provider, string error)
    {
        var (processor, emails) = Create();

        var result = processor.Process(Request(provider, amount, currency, card));

        Assert.Equal(new PaymentResult(false, null, 0m, error), result);
        Assert.Empty(emails);
    }
    [Fact]
    public void New_provider_needs_no_processor_change()
    {
        var emails = new InMemoryEmailSender();
        var processor = new PaymentProcessor(new PaymentValidator(), new IPaymentProvider[] { new AlwaysApproves() }, emails);

        var result = processor.Process(Request(provider: "test"));

        Assert.Equal(new PaymentResult(true, "test_1", 1.00m, null), result);
    }

    private sealed class AlwaysApproves : IPaymentProvider
    {
        public string Name => "test";
        public decimal FeeFor(decimal amount) => 1.00m;
        public ChargeOutcome Charge(decimal amount, string currency, string card) => new(true, "test_1");
    }
}
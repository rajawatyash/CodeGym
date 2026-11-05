using System.Globalization;

namespace Gym.Day07;


public interface IEmailSender
{
    void Send(PaymentRequest r);
}

public sealed class InMemoryEmailSender : IEmailSender
{
    public List<string> Sent { get; } = new();
    public void Send(PaymentRequest r) => Sent.Add($"To {r.CustomerEmail}: you paid {r.Amount.ToString("0.00", CultureInfo.InvariantCulture)} {r.Currency}");
}
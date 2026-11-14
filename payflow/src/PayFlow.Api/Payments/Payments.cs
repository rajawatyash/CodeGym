// PAYMENTS (Payments.cs, step 1)
using PayFlow.Domain;

namespace PayFlow.Api.Payments;

public enum PaymentStatus { Pending, Canceled }

public record Payment(string Id, Money Amount, string? Description, PaymentStatus Status);

public record CreatePaymentRequest(long Amount, string Currency, string? Description);

public sealed class PaymentService
{
    public Payment Create(long amountMinor, string currency, string? description) =>
        throw new NotImplementedException();

    public bool TryGet(string id, out Payment? payment) =>
        throw new NotImplementedException();
}
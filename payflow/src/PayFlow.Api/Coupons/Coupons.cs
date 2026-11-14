namespace PayFlow.Api.Coupons;

public record Coupon(string Code, int PercentOff);

public record CreateCouponRequest(string Code, int PercentOff);

public record CouponResponse(string Code, int PercentOff)
{
    public static CouponResponse From(Coupon coupon) => new(coupon.Code, coupon.PercentOff);
}

public sealed class DuplicateCouponException : InvalidOperationException
{
    public DuplicateCouponException(string code)
        : base($"Coupon '{code}' already exists.") => Code = code;

    public string Code { get; }
}

public interface ICouponStore
{
    bool TryGet(string code, out Coupon? coupon);
    void Add(Coupon coupon);
}

public sealed class InMemoryCouponStore : ICouponStore
{
    private readonly Dictionary<string, Coupon> _byCode = new(StringComparer.Ordinal);

    public bool TryGet(string code, out Coupon? coupon) => _byCode.TryGetValue(code, out coupon);

    public void Add(Coupon coupon) => _byCode.Add(coupon.Code, coupon);
}

public sealed class CouponBook
{
    private readonly ICouponStore _store;

    public CouponBook(ICouponStore store) =>
        _store = store ?? throw new ArgumentNullException(nameof(store));

    public Coupon Create(string code, int percentOff)
    {
        string normalized = NormalizeCode(code);
        if (percentOff is < 1 or > 100)
            throw new ArgumentOutOfRangeException(nameof(percentOff), percentOff, "Percent off must be 1 to 100.");
        if (_store.TryGet(normalized, out _))
            throw new DuplicateCouponException(normalized);

        var coupon = new Coupon(normalized, percentOff);
        _store.Add(coupon);
        return coupon;
    }

    public bool TryFind(string code, out Coupon? coupon)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            coupon = null;
            return false;
        }
        return _store.TryGet(code.Trim().ToUpperInvariant(), out coupon);
    }

    private static string NormalizeCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code is required.", nameof(code));

        string trimmed = code.Trim();
        if (trimmed.Length is < 3 or > 20)
            throw new ArgumentException("Code must be 3 to 20 characters.", nameof(code));
        foreach (char c in trimmed)
        {
            if (!char.IsAsciiLetterOrDigit(c))
                throw new ArgumentException("Code may only contain letters and digits.", nameof(code));
        }
        return trimmed.ToUpperInvariant();
    }
}
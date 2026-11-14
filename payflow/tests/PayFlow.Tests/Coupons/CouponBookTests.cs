using PayFlow.Api.Coupons;

namespace PayFlow.Tests.Coupons;

public class CouponBookTests
{
    private static CouponBook NewBook() => new(new InMemoryCouponStore());

    [Fact]
    public void Create_stores_the_code_in_capitals()
    {
        var book = NewBook();

        var coupon = book.Create("  save10 ", 10);

        Assert.Equal(new Coupon("SAVE10", 10), coupon);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("AB")]                       // too short
    [InlineData("ABCDEFGHIJKLMNOPQRSTU")]    // 21 characters
    [InlineData("SAVE-10")]                  // a dash
    [InlineData("SAVÉ10")]                   // not ASCII
    public void Create_rejects_bad_codes(string? code)
    {
        var book = NewBook();

        Assert.Throws<ArgumentException>(() => book.Create(code!, 10));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    [InlineData(-5)]
    public void Create_rejects_percent_outside_1_to_100(int percent)
    {
        var book = NewBook();

        Assert.Throws<ArgumentOutOfRangeException>(() => book.Create("SAVE10", percent));
    }

    [Fact]
    public void Create_rejects_the_same_code_in_another_case()
    {
        var book = NewBook();
        book.Create("SAVE10", 10);

        var ex = Assert.Throws<DuplicateCouponException>(() => book.Create("save10", 20));

        Assert.Equal("SAVE10", ex.Code);
        Assert.True(book.TryFind("SAVE10", out var kept));
        Assert.Equal(10, kept!.PercentOff);            // the first one is unchanged
    }

    [Fact]
    public void TryFind_works_in_any_case()
    {
        var book = NewBook();
        book.Create("SAVE10", 10);

        Assert.True(book.TryFind("save10", out var coupon));
        Assert.Equal("SAVE10", coupon!.Code);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("NOPE")]
    public void TryFind_returns_false_when_missing(string? code)
    {
        var book = NewBook();

        Assert.False(book.TryFind(code!, out var coupon));
        Assert.Null(coupon);
    }
}
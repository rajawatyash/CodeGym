using PayFlow.Api.Coupons;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// One CouponBook for the whole app, shared by every request.
// The lambdas below can use it because they "capture" it. P03 replaces this with dependency injection.
var coupons = new CouponBook(new InMemoryCouponStore());

app.MapGet("/health", () => "ok");

app.MapPost("/coupons", (CreateCouponRequest request) =>
{
    try
    {
        var coupon = coupons.Create(request.Code, request.PercentOff);
        return Results.Created($"/coupons/{coupon.Code}", CouponResponse.From(coupon));
    }
    catch (DuplicateCouponException ex)
    {
        return Results.Conflict(new { error = ex.Message });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapGet("/coupons/{code}", (string code) =>
    coupons.TryFind(code, out var coupon)
        ? Results.Ok(CouponResponse.From(coupon!))
        : Results.NotFound());

app.Run();
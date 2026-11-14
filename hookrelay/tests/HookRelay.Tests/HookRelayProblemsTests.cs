using HookRelay.Api.Destinations;
using Microsoft.AspNetCore.Http;

namespace HookRelay.Tests.Destinations;

public class HookRelayProblemsTests
{
    [Fact]
    public void Duplicate_becomes_409_naming_the_existing_destination()
    {
        var ex = new DuplicateDestinationException(
            "https://shop.kettle.co/hooks", "dst_1", DestinationStatus.Disabled);

        var p = HookRelayProblems.From(ex);

        Assert.NotNull(p);
        Assert.Equal(409, p!.Status);
        Assert.Equal("https://hookrelay.dev/problems/duplicate-destination", p.Type);
        Assert.Equal("Destination already registered", p.Title);
        Assert.Equal(ex.Message, p.Detail);
        Assert.Equal("dst_1", p.Extensions["existingId"]);
        Assert.Equal("Disabled", p.Extensions["existingStatus"]);
    }

    [Fact]
    public void Already_disabled_becomes_409_destination()
    {
        var ex = new DestinationAlreadyDisabledException("dst_1");

        var p = HookRelayProblems.From(ex);

        Assert.NotNull(p);
        Assert.Equal(409, p!.Status);
        Assert.Equal("Destination already disabled", p.Title);
        Assert.Equal(ex.Message, p.Detail);
        Assert.Equal("dst_1", p.Extensions["id"]);
    }

    [Fact]
    public void Validation_becomes_400_with_the_field_errors()
    {
        var errors = new Dictionary<string, string[]> { ["name"] = new[] { "Name is required." } };
        var ex = new DestinationValidationException(errors);

        var p = HookRelayProblems.From(ex);

        var v = Assert.IsType<HttpValidationProblemDetails>(p);    // the errors live on p, not ex
        Assert.Equal(400, v.Status);
        Assert.Equal("https://hookrelay.dev/problems/invalid-destination", v.Type);
        Assert.Equal("Invalid destination", v.Title);
        Assert.Equal(new[] { "name" }, v.Errors.Keys);
        Assert.Equal(new[] { "Name is required." }, v.Errors["name"]);
    }

    [Fact]
    public void Not_found_becomes_404_with_the_id()
    {
        var ex = new DestinationNotFoundException("dst_1");

        var p = HookRelayProblems.From(ex);

        Assert.NotNull(p);
        Assert.Equal(404, p!.Status);
        Assert.Equal("https://hookrelay.dev/problems/destination-not-found", p.Type);
        Assert.Equal("Destination not found", p.Title);
        Assert.Equal(ex.Message, p.Detail);
        Assert.Equal("dst_1", p.Extensions["id"]);
    }

    [Fact]
    public void Store_unavailable_becomes_503_without_the_inner_message()
    {
        var ex = new DestinationStoreUnavailableException(new IOException("disk full"));

        var p = HookRelayProblems.From(ex);

        Assert.NotNull(p);
        Assert.Equal(503, p!.Status);
        Assert.Equal("https://hookrelay.dev/problems/store-unavailable", p.Type);
        Assert.DoesNotContain("disk full", p.Detail);                // the cause stays in the logs
    }

    public static TheoryData<Exception> NotOurs => new()
    {
        new ArgumentException("from a bug somewhere"),
        new KeyNotFoundException(),
        new InvalidOperationException("Sequence contains no matching element"),
        new NullReferenceException(),
    };

    [Theory]
    [MemberData(nameof(NotOurs))]
    public void Exceptions_we_dont_own_are_not_mapped(Exception exception)
    {
        Assert.Null(HookRelayProblems.From(exception));
    }
    
}
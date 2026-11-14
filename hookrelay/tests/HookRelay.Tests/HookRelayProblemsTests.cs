using HookRelay.Api.Destinations;

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
    public void Invalid_Destination_name_400_destination()
    {
        Dictionary<string, string[]> errors = new Dictionary<string, string[]> { {"name",  new[]{"Name is required."}}};
        var ex = new DestinationValidationException(errors);

        var p = HookRelayProblems.From(ex);

        Assert.NotNull(p);
        Assert.Equal(400, p!.Status);
        Assert.Equal("Invalid destination", p.Title);
        Assert.Equal(ex.Message, p.Detail);
        Assert.Equal(new[] { "name"}, ex.Errors.Keys.OrderBy(k => k));
    }

    [Fact]
    public void Invalid_Destination_name_404_destination()
    {
        var ex = new DestinationNotFoundException("dst_1");

        var p = HookRelayProblems.From(ex);

        Assert.NotNull(p);
        Assert.Equal(404, p!.Status);
        Assert.Equal("Destination not found", p.Title);
        Assert.Equal(ex.Message, p.Detail);
        Assert.Equal("dst_1", p.Extensions["id"]);
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
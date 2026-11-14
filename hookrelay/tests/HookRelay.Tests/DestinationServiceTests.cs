using HookRelay.Api.Destinations;
using Microsoft.Extensions.Time.Testing;

namespace HookRelay.Tests.Destinations;

public class DestinationServiceTests
{
    private static readonly DateTimeOffset Start = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private static DestinationService NewService() =>
        new(new InMemoryDestinationStore(), new FakeIdGenerator(), new FakeSecretGenerator(),
            new FakeTimeProvider(Start));

    private static DestinationService FailingStoreService() =>
        new(new FailingDestinationStore(), new FakeIdGenerator(), new FakeSecretGenerator(),
            new FakeTimeProvider(Start));

    [Fact]
    public void Create_registers_an_active_destination_with_an_id_and_a_secret()
    {
        var service = NewService();

        var d = service.Create("  Kettle shop ", " https://Shop.Kettle.co/hooks/stripe ", "stripe");

        Assert.Equal("dst_1", d.Id);
        Assert.Equal("Kettle shop", d.Name);
        Assert.Equal("https://shop.kettle.co/hooks/stripe", d.Url.AbsoluteUri);
        Assert.Equal(SourceType.Stripe, d.Source);
        Assert.Equal("whsec_test_1", d.SigningSecret);
        Assert.Equal(DestinationStatus.Active, d.Status);
        Assert.Equal(Start, d.CreatedAt);                        // time comes from TimeProvider
    }

    [Fact]
    public void Each_destination_gets_its_own_id_and_secret()
    {
        var service = NewService();

        var first = service.Create("A", "https://a.example.com/hooks", "github");
        var second = service.Create("B", "https://b.example.com/hooks", "github");

        Assert.Equal("dst_2", second.Id);
        Assert.NotEqual(first.SigningSecret, second.SigningSecret);
    }

    [Fact]
    public void Http_is_allowed_only_for_localhost()
    {
        var service = NewService();

        var local = service.Create("Local", "http://localhost:5000/hooks", "generic");

        Assert.Equal("http://localhost:5000/hooks", local.Url.AbsoluteUri);
        Assert.Throws<DestinationValidationException>(() => service.Create("Shop", "http://shop.kettle.co/hooks", "generic"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    public void Create_requires_a_name(string? name)
    {
        var service = NewService();

        Assert.Throws<DestinationValidationException>(() => service.Create(name!, "https://a.example.com/h", "stripe"));
    }

    [Fact]
    public void Create_rejects_a_name_over_100_characters()
    {
        var service = NewService();

        Assert.Throws<DestinationValidationException>(() => service.Create(new string('x', 101), "https://a.example.com/h", "stripe"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData(" ")]
    [InlineData("shop.kettle.co/hooks")]                    // not absolute
    [InlineData("ftp://shop.kettle.co/hooks")]              // wrong scheme
    [InlineData("javascript:alert(1)")]                     // wrong scheme
    [InlineData("https://169.254.169.254/latest/meta-data")]// an IP address
    [InlineData("https://10.0.0.5/hooks")]                  // an IP address
    [InlineData("https://user:pass@shop.kettle.co/hooks")]  // credentials in the URL
    public void Create_rejects_unsafe_or_invalid_urls(string? url)
    {
        var service = NewService();

        Assert.Throws<DestinationValidationException>(() => service.Create("Shop", url!, "stripe"));
    }

    [Fact]
    public void Create_rejects_a_url_already_registered_ignoring_host_case()
    {
        var service = NewService();
        service.Create("Shop", "https://Shop.Kettle.co/hooks", "stripe");

        var ex = Assert.Throws<DuplicateDestinationException>(() =>
            service.Create("Shop again", "https://shop.kettle.co/hooks", "stripe"));

        Assert.Equal("https://shop.kettle.co/hooks", ex.Url);
    }

    [Fact]
    public void The_path_is_case_sensitive()
    {
        var service = NewService();
        service.Create("Lower", "https://shop.kettle.co/hooks", "stripe");

        var upper = service.Create("Upper", "https://shop.kettle.co/HOOKS", "stripe");   // a different URL

        Assert.Equal("dst_2", upper.Id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("paypal")]
    [InlineData("0")]
    [InlineData("stripe,github")]
    public void Create_rejects_unknown_sources(string? source)
    {
        var service = NewService();

        Assert.Throws<DestinationValidationException>(() => service.Create("Shop", "https://a.example.com/h", source!));
    }

    [Fact]
    public void A_rejected_destination_is_not_stored_and_uses_no_id_or_secret()
    {
        var service = NewService();
        Assert.Throws<DestinationValidationException>(() => service.Create("Shop", "https://a.example.com/h", "paypal"));

        var d = service.Create("Shop", "https://a.example.com/h", "stripe");   // same URL works: nothing was stored

        Assert.Equal("dst_1", d.Id);                                            // and nothing was used up
        Assert.Equal("whsec_test_1", d.SigningSecret);
    }

    [Fact]
    public void TryGet_finds_a_registered_destination()
    {
        var service = NewService();
        var created = service.Create("Shop", "https://a.example.com/h", "stripe");

        Assert.True(service.TryGet("dst_1", out var found));
        Assert.Equal(created, found);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("dst_404")]
    public void TryGet_returns_false_when_missing(string? id)
    {
        var service = NewService();

        Assert.False(service.TryGet(id!, out var destination));
        Assert.Null(destination);
    }

    private static DestinationService ServiceWithThree()
    {
        var service = NewService();
        service.Create("Shop", "https://a.example.com/h", "stripe");   // dst_1
        service.Create("Repo", "https://b.example.com/h", "github");   // dst_2
        service.Create("Orders", "https://c.example.com/h", "github");   // dst_3
        return service;
    }

    [Fact]
    public void List_without_a_source_returns_all_in_registration_order()
    {
        var list = ServiceWithThree().GetDestinations(null);

        Assert.Equal(new[] { "dst_1", "dst_2", "dst_3" }, list.Select(d => d.Id));
    }

    [Fact]
    public void List_with_a_source_returns_only_that_source_in_order()
    {
        var list = ServiceWithThree().GetDestinations("GitHub");

        Assert.Equal(new[] { "dst_2", "dst_3" }, list.Select(d => d.Id));
    }

    [Theory]
    [InlineData("paypal")]
    [InlineData("0")]
    [InlineData("stripe,github")]
    public void List_rejects_an_unknown_source(string source)
    {
        Assert.Throws<DestinationValidationException>(() => ServiceWithThree().GetDestinations(source));
    }

    [Fact]
    public void Disable_an_unknown_id_throws_not_found()
    {
        Assert.Throws<DestinationNotFoundException>(() => ServiceWithThree().DisableDestination("dst_404"));
    }

    [Fact]
    public void Disable_an_active_destination_returns_it_disabled()
    {
        var d = ServiceWithThree().DisableDestination("dst_2");

        Assert.Equal("dst_2", d.Id);
        Assert.Equal(DestinationStatus.Disabled, d.Status);
    }

    [Fact]
    public void Disable_is_saved()
    {
        var service = ServiceWithThree();
        service.DisableDestination("dst_2");

        Assert.True(service.TryGet("dst_2", out var found));
        Assert.Equal(DestinationStatus.Disabled, found.Status);
        Assert.Equal(DestinationStatus.Disabled, service.GetDestinations("github")[0].Status); // the list copy too
    }

    [Fact]
    public void Disable_twice_throws_conflict()
    {
        var service = ServiceWithThree();
        service.DisableDestination("dst_2");

        Assert.Throws<DestinationAlreadyDisabledException>(() => service.DisableDestination("dst_2"));
    }

    [Fact]
    public void Registering_a_disabled_url_again_is_a_conflict_that_names_the_existing_destination()
    {
        var service = NewService();
        service.Create("Shop", "https://shop.kettle.co/hooks", "stripe");   // dst_1
        service.DisableDestination("dst_1");

        var ex = Assert.Throws<DuplicateDestinationException>(() =>
            service.Create("Shop again", "https://shop.kettle.co/hooks", "stripe"));

        Assert.Equal("dst_1", ex.ExistingId);
        Assert.Contains("Disabled", ex.Message);                 // tells the caller why: enable it instead
        Assert.False(service.TryGet("dst_2", out _));            // nothing new was created
    }

    [Fact]
    public void Every_bad_field_is_reported_at_once()
    {
        var ex = Assert.Throws<DestinationValidationException>(() =>
            NewService().Create("", "https://a.example.com/h", "paypal"));

        Assert.Equal(new[] { "name", "source" }, ex.Errors.Keys.OrderBy(k => k));
    }

    [Fact]
    public void A_store_failure_is_wrapped_and_keeps_the_cause()
    {
        var ex = Assert.Throws<DestinationStoreUnavailableException>(() =>
            FailingStoreService().Create("Shop", "https://a.example.com/h", "github"));

        Assert.IsType<IOException>(ex.InnerException);
    }
}
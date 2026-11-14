using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using HookRelay.Api.Destinations;
using HookRelay.Tests.Destinations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HookRelay.Tests;

// ONE app (and so ONE in-memory store) is shared by every test in this class (IClassFixture).
// So every test owns its data:
//   - a unique URL per test (NewUrl), never a fixed one;
//   - ids are read from responses, never assumed to be dst_1;
//   - no assertion depends on what other tests stored ("exactly 2").
// Tests that need a different app (a failing store) build their own with WithWebHostBuilder.
public sealed class DestinationsApiTests : IClassFixture<HookRelayFactory>
{
    private static readonly DateTimeOffset FakeNow = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
    private const string Problems = "https://hookrelay.dev/problems/";

    private readonly HookRelayFactory _factory;
    private readonly HttpClient _client;

    public DestinationsApiTests(HookRelayFactory factory)   // xUnit passes the shared one
    {
        _factory = factory;
        _client = factory.CreateClient();
    }   // no Dispose of the factory: xUnit owns it

    private static string NewUrl() => $"https://hooks.kettle.co/{Guid.NewGuid():N}";

    private async Task<CreateDestinationResponse> CreateAsync(string name, string url, string source)
    {
        var response = await _client.PostAsJsonAsync("/destinations", new { name, url, source });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);   // setup must really succeed
        return (await response.Content.ReadFromJsonAsync<CreateDestinationResponse>())!;
    }

    private static async Task<JsonElement> ProblemAsync(HttpResponseMessage response)
    {
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return doc.RootElement.Clone();                              // usable after doc is disposed
    }

    [Fact]
    public async Task Create_returns_201_with_location_and_body()
    {
        var url = NewUrl();

        var response = await _client.PostAsJsonAsync("/destinations",
            new { name = "Kettle shop", url, source = "stripe" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<CreateDestinationResponse>();
        Assert.Equal($"/destinations/{body!.Id}", response.Headers.Location!.ToString());
        Assert.StartsWith("dst_", body.Id);
        Assert.Equal("Kettle shop", body.Name);
        Assert.Equal("Stripe", body.Source);
        Assert.Equal(url, body.Url);
        Assert.Equal("Active", body.Status);
        Assert.StartsWith("whsec_test_", body.SigningSecret);       // shown once, on create only
        Assert.Equal(FakeNow, body.CreatedAt);                       // fake clock from the factory
    }

    [Fact]
    public async Task Created_destination_can_be_read_back_without_its_secret()
    {
        var created = await CreateAsync("Kettle shop", NewUrl(), "stripe");

        var response = await _client.GetAsync($"/destinations/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("secret", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(created.SigningSecret, json);
        var body = JsonSerializer.Deserialize<GetDestinationResponse>(json, JsonSerializerOptions.Web);
        Assert.Equal(created.Id, body!.Id);
    }

    [Fact]
    public async Task Unknown_id_is_404_problem_details()
    {
        var response = await _client.GetAsync("/destinations/dst_nope");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await ProblemAsync(response);                                // checks problem+json
    }

    [Fact]
    public async Task Every_bad_field_is_a_400_with_all_errors()
    {
        var response = await _client.PostAsJsonAsync("/destinations",
            new { name = "", url = NewUrl(), source = "paypal" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await ProblemAsync(response);
        Assert.Equal(Problems + "invalid-destination", problem.GetProperty("type").GetString());
        var errors = problem.GetProperty("errors");
        Assert.True(errors.TryGetProperty("name", out _));
        Assert.True(errors.TryGetProperty("source", out _));
    }

    [Fact]
    public async Task Duplicate_url_is_409_naming_the_existing_destination()
    {
        var url = NewUrl();
        var first = await CreateAsync("Kettle shop", url, "stripe");

        var response = await _client.PostAsJsonAsync("/destinations",
            new { name = "Copy", url, source = "github" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var problem = await ProblemAsync(response);
        Assert.Equal(Problems + "duplicate-destination", problem.GetProperty("type").GetString());
        Assert.Equal(first.Id, problem.GetProperty("existingId").GetString());
        Assert.Equal("Active", problem.GetProperty("existingStatus").GetString());
    }

    [Fact]
    public async Task Disable_then_disable_again_is_409()
    {
        var created = await CreateAsync("Kettle shop", NewUrl(), "stripe");

        var first = await _client.PostAsync($"/destinations/{created.Id}/disable", null);
        var second = await _client.PostAsync($"/destinations/{created.Id}/disable", null);

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
        var problem = await ProblemAsync(second);
        Assert.Equal(Problems + "destination-already-disabled", problem.GetProperty("type").GetString());
        Assert.Equal(created.Id, problem.GetProperty("id").GetString());
    }

    [Fact]
    public async Task Disable_unknown_id_is_404()
    {
        var response = await _client.PostAsync("/destinations/dst_nope/disable", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problem = await ProblemAsync(response);
        Assert.Equal(Problems + "destination-not-found", problem.GetProperty("type").GetString());
    }

    [Fact]
    public async Task List_by_source_returns_only_that_source_in_creation_order()
    {
        var a = await CreateAsync("Shop A", NewUrl(), "stripe");
        var b = await CreateAsync("Shop B", NewUrl(), "stripe");
        var c = await CreateAsync("Repo", NewUrl(), "github");

        var list = await _client.GetFromJsonAsync<List<GetDestinationResponse>>(
            "/destinations?source=stripe");

        // other tests may have added stripe destinations too: check OURS, not the count
        var ids = list!.Select(d => d.Id).ToList();
        Assert.Contains(a.Id, ids);
        Assert.Contains(b.Id, ids);
        Assert.True(ids.IndexOf(a.Id) < ids.IndexOf(b.Id));           // creation order
        Assert.DoesNotContain(c.Id, ids);
        Assert.All(list, d => Assert.Equal("Stripe", d.Source));
    }

    [Fact]
    public async Task Store_down_is_503_with_retry_after_and_no_internal_detail()
    {
        await using var factory = _factory.WithWebHostBuilder(b => b.ConfigureTestServices(s =>
        {
            s.RemoveAll<IDestinationStore>();
            s.AddSingleton<IDestinationStore, FailingDestinationStore>();
        }));
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/destinations",
            new { name = "Kettle shop", url = NewUrl(), source = "stripe" });

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Equal(TimeSpan.FromSeconds(5), response.Headers.RetryAfter?.Delta);
        var text = await response.Content.ReadAsStringAsync();
        Assert.Contains(Problems + "store-unavailable", text);
        Assert.DoesNotContain("disk full", text);
    }

    [Fact]
    public async Task Unknown_failure_is_a_500_that_leaks_nothing()
    {
        await using var factory = _factory.WithWebHostBuilder(b =>
        {
            b.UseEnvironment("Production");      // what customers get: no developer details
            b.ConfigureTestServices(s =>
            {
                s.RemoveAll<IDestinationStore>();
                s.AddSingleton<IDestinationStore, ExplodingDestinationStore>();
            });
        });
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/destinations",
            new { name = "Kettle shop", url = NewUrl(), source = "stripe" });

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        var text = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("hunter2", text);
        Assert.DoesNotContain("10.0.0.5", text);
    }
}

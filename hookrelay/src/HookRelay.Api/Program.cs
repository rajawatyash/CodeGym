using HookRelay.Api.Destinations;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider(o => 
{
    o.ValidateScopes = true;
    o.ValidateOnBuild = true;
});
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<DestinationExceptionHandler>();

// Lifetimes (P03).
// a ready-made instance (TimeProvider is abstract, so it can't be constructed by the container)
builder.Services.AddSingleton(TimeProvider.System);
// singleton: the in-memory store IS the data. Scoped or transient would give each request
// a new, empty store (POST 201, then GET 404) once anything that uses it isn't a singleton.
builder.Services.AddSingleton<IDestinationStore, InMemoryDestinationStore>();
// singleton: stateless and thread-safe (Guid.NewGuid), so one instance is enough
builder.Services.AddSingleton<IIdGenerator, GuidIdGenerator>();
builder.Services.AddSingleton<ISecretGenerator, GuidSecretGenerator>();
// singleton: no state of its own, and depends only on singletons (no captive dependency)
builder.Services.AddSingleton<DestinationService>();

var app = builder.Build();
app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapGet("/health", () => "ok");

app.MapPost("/destinations", (CreateDestinationRequest request, DestinationService destinationService) =>
{
    var value = destinationService.Create(request.Name, request.Url, request.Source);

    return Results.Created($"/destinations/{value.Id}", CreateDestinationResponse.From(value));
    
});

app.MapGet("/destinations/{id}", (string id, DestinationService destinationService) =>
    destinationService.TryGet(id, out var destination)
        ? Results.Ok(GetDestinationResponse.From(destination))
        : Results.NotFound());

app.MapGet("/destinations", (string? source, DestinationService destinationService) =>
{
    var list = destinationService.GetDestinations(source)
            .Select(GetDestinationResponse.From)
            .ToList();

    return Results.Ok(list);
});

app.MapPost("/destinations/{id}/disable", (string id, DestinationService destinationService) =>
{
    var value = destinationService.DisableDestination(id);

    return Results.Ok(GetDestinationResponse.From(value));
    
});
    

app.Run();

public partial class Program { }   // lets the test project name Program
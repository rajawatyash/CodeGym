using HookRelay.Api.Destinations;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<DestinationExceptionHandler>();

var app = builder.Build();
app.UseExceptionHandler();
app.UseStatusCodePages();

var idGenerator = new GuidIdGenerator();
var secretGenerator = new GuidSecretGenerator();
var inMemoryStorage = new InMemoryDestinationStore();

var destinationService = new DestinationService(inMemoryStorage, idGenerator, secretGenerator);

app.MapGet("/health", () => "ok");

app.MapPost("/destinations", (CreateDestinationRequest request) =>
{
    var value = destinationService.Create(request.Name, request.Url, request.Source);

    return Results.Created($"/destinations/{value.Id}", CreateDestinationResponse.From(value));
    
});

app.MapGet("/destinations/{id}", (string id) =>
    destinationService.TryGet(id, out var destination)
        ? Results.Ok(GetDestinationResponse.From(destination))
        : Results.NotFound());

app.MapGet("/destinations", (string? source) =>
{
    var list = destinationService.GetDestinations(source)
            .Select(GetDestinationResponse.From)
            .ToList();

    return Results.Ok(list);
});

app.MapPost("/destinations/{id}/disable", (string id) =>
{
    var value = destinationService.DisableDestination(id);

    return Results.Ok(GetDestinationResponse.From(value));
    
});
    

app.Run();
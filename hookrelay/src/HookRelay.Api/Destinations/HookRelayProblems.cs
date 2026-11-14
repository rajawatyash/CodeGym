using Microsoft.AspNetCore.Mvc;

namespace HookRelay.Api.Destinations;

public static class HookRelayProblems
{
    private const string Base = "https://hookrelay.dev/problems/";

    public static ProblemDetails? From(Exception exception) => exception switch
    {
        DuplicateDestinationException e => new ProblemDetails
        {
            Type = Base + "duplicate-destination",
            Title = "Destination already registered",
            Status = StatusCodes.Status409Conflict,
            Detail = e.Message,
            Extensions = { 
                ["existingId"] = e.ExistingId,
                ["existingStatus"] = e.ExistingStatus.ToString()   // "Disabled", not 1
            }
        },
        DestinationAlreadyDisabledException e => new ProblemDetails
        {
            Type = Base + "destination-already-disabled",
            Title = "Destination already disabled",
            Status = StatusCodes.Status409Conflict,
            Detail = e.Message,
            Extensions = { ["id"] = e.Id }
        },
        DestinationValidationException e => new HttpValidationProblemDetails(e.Errors)
        {
            Type = Base + "invalid-destination",
            Title = "Invalid destination",
            Status = StatusCodes.Status400BadRequest,
            Detail = e.Message
        },
        DestinationNotFoundException e => new ProblemDetails
        {
            Type = Base + "destination-not-found",
            Title = "Destination not found",
            Status = StatusCodes.Status404NotFound,
            Detail = e.Message,
            Extensions = { ["id"] = e.Id }
        },
        DestinationStoreUnavailableException  e => new ProblemDetails
        {
            Type = Base + "store-unavailable",
            Title = "Destination store unavailable",
            Status = StatusCodes.Status503ServiceUnavailable,
            Detail = e.Message,
        },
        _ => null   // not ours: the default handling turns it into a 500
    };
}
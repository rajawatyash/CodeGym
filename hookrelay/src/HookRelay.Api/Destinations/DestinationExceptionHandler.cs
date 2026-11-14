using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;

namespace HookRelay.Api.Destinations;

public sealed class DestinationExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problems;
    private readonly ILogger<DestinationExceptionHandler> _logger;


    public DestinationExceptionHandler(IProblemDetailsService problems, ILogger<DestinationExceptionHandler> logger)
    {
        _problems = problems;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
                                                CancellationToken cancellationToken)
    {
        string traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;   // same id as the response
        var problem = HookRelayProblems.From(exception);
        if (problem is null)
        {
            _logger.LogError(exception, "Unhandled exception. TraceId {TraceId}", traceId);
            return false; // not ours → generic 500
        }
        if(problem.Status!.Value >= 500)
        {
            // our failure (e.g. 503): Error, with the exception so the inner cause is in the log
            _logger.LogError(exception, "Request failed with {Status} {Type}. TraceId {TraceId}",
                             problem.Status, problem.Type, traceId);
        }
        else
        {
            _logger.LogInformation("Request rejected with {Status} {Type}. TraceId {TraceId}",
                               problem.Status, problem.Type, traceId);
        }
        
        
        httpContext.Response.StatusCode = problem.Status!.Value;
        if (exception is DestinationStoreUnavailableException)
            httpContext.Response.Headers.RetryAfter = "5";
        return await _problems.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problem,
            Exception = exception
        });
    }
}
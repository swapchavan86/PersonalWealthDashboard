using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
namespace PersonalWealth.Api.Infrastructure;
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception,"Unhandled request failure");
        var status = exception is ArgumentException or ArgumentOutOfRangeException ? StatusCodes.Status400BadRequest : StatusCodes.Status500InternalServerError;
        await Results.Problem(statusCode:status,title:status==400?"Validation failed":"Request failed",detail:status==400?exception.Message:"An unexpected error occurred.").ExecuteAsync(httpContext);
        return true;
    }
}

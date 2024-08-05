using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using TrainStationsRepo.API.Models;

namespace TrainStationsRepo.API.Services;

class ExceptionFallback(ILogger<ExceptionFallback> logger): IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Exception during request handling");
        
        httpContext.Response.ContentType = MediaTypeNames.Application.Json;
        httpContext.Response.WriteAsJsonAsync(new ErrorModel("Something went wrong", ErrorCodes.SWW), cancellationToken: cancellationToken);
        
        return ValueTask.FromResult(true);
    }
}
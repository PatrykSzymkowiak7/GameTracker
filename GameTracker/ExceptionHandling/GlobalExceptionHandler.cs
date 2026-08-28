using GameTracker.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GameTracker.Api.ExceptionHandling
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext, 
            Exception exception, 
            CancellationToken cancellationToken)
        {
            if(!(exception is GameNotFoundException) && 
                !(exception is GameConflictException) &&
                 !(exception is DeveloperNotFoundException) &&
                  !(exception is GameValidationException))
            {
                _logger.LogError(
                    exception,
                    "An unhandled exception ocurred.");
            }

            var statusCode = exception switch
            {
                GameNotFoundException => StatusCodes.Status404NotFound,
                DeveloperNotFoundException => StatusCodes.Status404NotFound,
                GameValidationException => StatusCodes.Status400BadRequest,
                GameConflictException => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

            var title = exception switch
            { 
                GameNotFoundException => "Game not found",
                DeveloperNotFoundException => "Developer not found",
                GameValidationException => "Validation error",
                GameConflictException => "Game conflict",
                _ => "Internal server error"
            };

            var detail = exception switch
            {
                GameNotFoundException => exception.Message,
                DeveloperNotFoundException => exception.Message,
                GameConflictException => exception.Message,
                _ => "An unhandlex exception occured."
            };

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail
            };

            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsJsonAsync(
                problemDetails, 
                cancellationToken);

            return true;
        }
    }
}

using GameTracker.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GameTracker.Api.ExceptionHandling
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        private List<Type> HandledExceptions = new List<Type>()
        {
            typeof(GameNotFoundException),
            typeof(GameConflictException),
            typeof(DeveloperNotFoundException),
            typeof(GameValidationException),
            typeof(InvalidCredentialsException),
            typeof(RegistrationException)
        };

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext, 
            Exception exception, 
            CancellationToken cancellationToken)
        {
            if(!HandledExceptions.Contains(typeof(Exception)))
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
                RegistrationException => StatusCodes.Status400BadRequest,
                InvalidCredentialsException => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError
            };

            var title = exception switch
            { 
                GameNotFoundException => "Game not found",
                DeveloperNotFoundException => "Developer not found",
                GameValidationException => "Validation error",
                GameConflictException => "Game conflict",
                RegistrationException => "Registration failed",
                InvalidCredentialsException => "Invalid credentials",
                _ => "Internal server error"
            };

            var detail = exception switch
            {
                GameNotFoundException => exception.Message,
                DeveloperNotFoundException => exception.Message,
                GameConflictException => exception.Message,
                InvalidCredentialsException => exception.Message,
                RegistrationException => exception.Message,
                _ => "An unhandlex exception occured."
            };

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail
            };

            if(exception is RegistrationException registrationException)
            {
                problemDetails.Extensions["errors"] =
                    registrationException.Errors;
            }

            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsJsonAsync(
                problemDetails, 
                cancellationToken);

            return true;
        }
    }
}

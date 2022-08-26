using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using STrain.Core.Enumerations;
using STrain.Core.Exceptions;

namespace STrain.CQS.NetCore.ErrorHandling
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                if (!context.Response.HasStarted)
                {
                    switch (context.Response.StatusCode)
                    {
                        case StatusCodes.Status401Unauthorized:
                            await context.Response.WriteUnathorizedAsync(ErrorEnumeration.Authentication.Unathorized, context.Request.Path, context.RequestAborted);
                            break;
                        case StatusCodes.Status404NotFound:
                            await context.Response.WriteAsync(ErrorEnumeration.NotFound.Endpoint, context.Request.Path, context.RequestAborted);
                            break;
                    }
                }
            }
            catch (NotFoundException exception)
            {
                _logger.LogError(exception, "Resource not found");
                await context.Response.WriteAsync(exception, $"{context.Request.Path};{exception.Request.GetType().Name}", context.RequestAborted);
            }
        }
    }

    internal static class ErrorHandlingMiddlewareExtensions
    {
        public static async Task WriteUnathorizedAsync(this HttpResponse response, ErrorEnumeration.Authentication enumeration, string instance, CancellationToken cancellationToken)
        {
            await response.WriteAsync(new ProblemDetails
            {
                Type = enumeration.Type,
                Title = enumeration.Title,
                Detail = string.Format(enumeration.Detail, instance),
                Instance = instance,
                Status = StatusCodes.Status401Unauthorized
            }, StatusCodes.Status401Unauthorized, cancellationToken);
        }

        public static async Task WriteAsync(this HttpResponse response, NotFoundException exception, string instance, CancellationToken cancellationToken)
        {
            await response.WriteAsync(new ProblemDetails
            {
                Type = exception.Type,
                Title = exception.Title,
                Detail = exception.Detail,
                Instance = instance,
                Status = StatusCodes.Status404NotFound
            }, StatusCodes.Status404NotFound, cancellationToken);
        }
        public static async Task WriteAsync(this HttpResponse response, ErrorEnumeration.NotFound enumeration, string instance, CancellationToken cancellationToken)
        {
            await response.WriteAsync(new ProblemDetails
            {
                Type = enumeration.Type,
                Title = enumeration.Title,
                Detail = string.Format(enumeration.Detail, instance),
                Instance = instance,
                Status = StatusCodes.Status404NotFound
            }, StatusCodes.Status404NotFound, cancellationToken);
        }
        public static async Task WriteAsync(this HttpResponse response, ProblemDetails problem, int statusCode, CancellationToken cancellationToken)
        {
            response.StatusCode = statusCode;
            await response.WriteAsJsonAsync(problem, null, MediaTypeNames.Application.Json.Problem, cancellationToken);
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
                            _logger.LogError("Unathorized request");
                            await context.Response.WriteAsProblemAsync(ErrorEnumeration.Unathorized, context.Request.Path, context.RequestAborted);
                            break;
                        case StatusCodes.Status403Forbidden:
                            _logger.LogError("Forbidden request");
                            await context.Response.WriteAsProblemAsync(ErrorEnumeration.Forbidden, context.Request.Path, context.RequestAborted);
                            break;
                        case StatusCodes.Status404NotFound:
                            _logger.LogError("Endpoint not found");
                            await context.Response.WriteAsProblemAsync(ErrorEnumeration.NotFound.Endpoint, context.Request.Path, context.RequestAborted);
                            break;
                    }
                }
            }
            catch (NotFoundException exception)
            {
                _logger.LogError(exception, "Resource not found");
                await context.Response.WriteAsProblemAsync(exception, context.Request.Path, context.RequestAborted);
            }
            catch (VerificationException exception)
            {
                _logger.LogError(exception, "Verification error");
                await context.Response.WriteAsProblemAsync(exception, context.Request.Path, context.RequestAborted);
            }
            catch (ValidationException exception)
            {
                _logger.LogError(exception, "Invalid request");
                await context.Response.WriteAsProblemAsync(exception, context.Request.Path, context.RequestAborted);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Internal server error");
                await context.Response.WriteAsProblemAsync(ErrorEnumeration.InternalServerError, context.Request.Path, context.RequestAborted);
            }
        }
    }

    internal static class ErrorHandlingMiddlewareExtensions
    {
        public static async Task WriteAsProblemAsync(this HttpResponse response, ValidationException exception, string instance, CancellationToken cancellationToken)
        {
            var problem = new ProblemDetails
            {
                Type = ErrorEnumeration.Validation.Type,
                Title = ErrorEnumeration.Validation.Title,
                Detail = ErrorEnumeration.Validation.Detail,
                Instance = instance,
                Status = ErrorEnumeration.Validation.Id
            };
            problem.Extensions.Add("Errors", exception.Errors.Select(e => new { Property = e.Key, Message = e.Value }));
            await response.WriteAsProblemAsync(problem, ErrorEnumeration.Validation.Id, cancellationToken);
        }

        public static async Task WriteAsProblemAsync(this HttpResponse response, VerificationException exception, string instance, CancellationToken cancellationToken)
        {
            await response.WriteAsProblemAsync(new ProblemDetails
            {
                Type = exception.Type,
                Title = exception.Title,
                Detail = exception.Detail,
                Instance = instance,
                Status = StatusCodes.Status400BadRequest
            }, StatusCodes.Status400BadRequest, cancellationToken);
        }

        public static async Task WriteAsProblemAsync(this HttpResponse response, ErrorEnumeration enumeration, string instance, CancellationToken cancellationToken)
        {
            await response.WriteAsProblemAsync(new ProblemDetails
            {
                Type = enumeration.Type,
                Title = enumeration.Title,
                Detail = string.Format(enumeration.Detail, instance),
                Instance = instance,
                Status = enumeration.Id
            }, enumeration.Id, cancellationToken);
        }

        public static async Task WriteAsProblemAsync(this HttpResponse response, NotFoundException exception, string instance, CancellationToken cancellationToken)
        {
            await response.WriteAsProblemAsync(new ProblemDetails
            {
                Type = exception.Type,
                Title = exception.Title,
                Detail = exception.Detail,
                Instance = instance,
                Status = StatusCodes.Status404NotFound
            }, StatusCodes.Status404NotFound, cancellationToken);
        }
        public static async Task WriteAsProblemAsync(this HttpResponse response, ErrorEnumeration.NotFound enumeration, string instance, CancellationToken cancellationToken)
        {
            await response.WriteAsProblemAsync(new ProblemDetails
            {
                Type = enumeration.Type,
                Title = enumeration.Title,
                Detail = string.Format(enumeration.Detail, instance),
                Instance = instance,
                Status = StatusCodes.Status404NotFound
            }, StatusCodes.Status404NotFound, cancellationToken);
        }
        public static async Task WriteAsProblemAsync(this HttpResponse response, ProblemDetails problem, int statusCode, CancellationToken cancellationToken)
        {
            response.StatusCode = statusCode;
            await response.WriteAsJsonAsync(problem, null, MediaTypeNames.Application.Json.Problem, cancellationToken);
        }
    }
}

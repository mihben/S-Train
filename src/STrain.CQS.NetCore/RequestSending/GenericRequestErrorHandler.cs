using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using STrain.Core.Exceptions;
using STrain.CQS.NetCore.ErrorHandling;
using STrain.CQS.Senders;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace STrain.CQS.NetCore.RequestSending
{
    public class GenericRequestErrorHandler : IRequestErrorHandler
    {
        private readonly ILogger<GenericRequestErrorHandler> _logger;

        public GenericRequestErrorHandler(ILogger<GenericRequestErrorHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Mapping HTTP error response ({StatusCode})", response.StatusCode);
            if (response.Content.Headers.ContentType is null
                || !response.Content.Headers.ContentType.Equals(MediaTypeNames.Application.Json.Problem)) throw RequestException(response.StatusCode);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: cancellationToken);
            if (problem is null) throw InvalidOperationException();

            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    throw new NotFoundException(problem.Type!, problem.Title!, problem.Detail!);
                case HttpStatusCode.BadRequest:
                    if (problem.Type?.Equals(ErrorEnumeration.Validation.Type, StringComparison.OrdinalIgnoreCase) ?? false)
                        throw problem.AsValidationException();
                    throw problem.AsVerificationException();
                default:
                    throw RequestException(response.StatusCode);
            }
        }

        private static HttpRequestException RequestException(HttpStatusCode statusCode) => new("Error during calling external service", null, statusCode);
        private static InvalidOperationException InvalidOperationException() => new("Error during reading response");
    }

    internal static class GenericResponseHandlerExtensions
    {
        public static ValidationException AsValidationException(this ProblemDetails problem)
        {
            if (problem.Extensions.TryGetValue("errors", out var errors))
                return new ValidationException(errors.AsReadOnlyDictionary());
            return new ValidationException();
        }

        public static VerificationException AsVerificationException(this ProblemDetails problem)
        {
            return new VerificationException(problem.Type!, problem.Title!, problem.Detail!);
        }

        public static IReadOnlyDictionary<string, string> AsReadOnlyDictionary(this object? errors)
        {
            if (errors is null) return new Dictionary<string, string>();

            return ((JsonElement)errors).Deserialize<IEnumerable<Error>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true })?.ToDictionary(e => e.Property, e => e.Message) ?? new Dictionary<string, string>();
        }
    }

    internal class Error
    {
        public string Property { get; }
        public string Message { get; }

        public Error(string property, string message)
        {
            Property = property;
            Message = message;
        }
    }
}

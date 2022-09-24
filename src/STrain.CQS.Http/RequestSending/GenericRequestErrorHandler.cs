using Microsoft.Extensions.Logging;
using STrain.Core.Exceptions;
using STrain.CQS.NetCore.ErrorHandling;
using STrain.CQS.Senders;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace STrain.CQS.Http.RequestSending
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
                || response.Content.Headers.ContentType.MediaType is null
                || !response.Content.Headers.ContentType.MediaType.Equals(MediaTypeNames.Application.Json.Problem, StringComparison.OrdinalIgnoreCase)) throw RequestException(response.StatusCode);

            var problem = await response.Content.ReadFromJsonAsync<Problem>(cancellationToken: cancellationToken);
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
        public static ValidationException AsValidationException(this Problem problem)
        {
            if (problem.Extensions?.TryGetValue("errors", out var errors) ?? false)
                return new ValidationException(errors.AsReadOnlyDictionary());
            return new ValidationException();
        }

        public static VerificationException AsVerificationException(this Problem problem)
        {
            return new VerificationException(problem.Type!, problem.Title!, problem.Detail!);
        }

        public static IReadOnlyDictionary<string, string> AsReadOnlyDictionary(this object? errors)
        {
            if (errors is null) return new Dictionary<string, string>();

            return ((JsonElement)errors).Deserialize<IEnumerable<Error>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true })?.ToDictionary(e => e.Property, e => e.Message) ?? new Dictionary<string, string>();
        }
    }

    public class Problem
    {
        public string? Title { get; init; }
        public string? Detail { get; init; }
        public string? Instance { get; init; }
        public string? Type { get; init; }
        public int? Status { get; init; }
        [JsonExtensionData]
        public IDictionary<string, object?>? Extensions { get; init; }
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

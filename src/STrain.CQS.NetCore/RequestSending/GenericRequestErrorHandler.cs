using Microsoft.AspNetCore.Mvc;
using STrain.Core.Exceptions;
using STrain.CQS.NetCore.ErrorHandling;
using STrain.CQS.Senders;
using System.Net;
using System.Net.Http.Json;

namespace STrain.CQS.NetCore.RequestSending
{
    public class GenericRequestErrorHandler : IRequestErrorHandler
    {
        public async Task HandleAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            if (response.Content.Headers.ContentType is null
                || response.Content.Headers.ContentType.Equals(MediaTypeNames.Application.Json.Problem)) throw RequestException(response.StatusCode);

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

        private HttpRequestException RequestException(HttpStatusCode statusCode) => new("Error during calling external service", null, statusCode);
        private InvalidOperationException InvalidOperationException() => new("Error during reading response");
    }

    internal static class GenericResponseHandlerExtensions
    {
        public static ValidationException AsValidationException(this ProblemDetails problem)
        {
            if (problem.Extensions.TryGetValue("errors", out var errors))
                return new ValidationException((errors as IEnumerable<(string Property, string Message)>)?.ToDictionary(e => e.Property, e => e.Message) ?? new Dictionary<string, string>());
            return new ValidationException();
        }

        public static VerificationException AsVerificationException(this ProblemDetails problem)
        {
            return new VerificationException(problem.Type!, problem.Title!, problem.Detail!);
        }
    }
}

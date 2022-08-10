using System.Text.Json;

namespace Microsoft.AspNetCore.Http
{
    public static class HttpRequestExtensions
    {
        public static async ValueTask<object?> ReadAsJsonAsync(this HttpRequest request, Type type, CancellationToken cancellationToken)
        {
            return await JsonSerializer.DeserializeAsync(request.Body, type, cancellationToken: cancellationToken);
        }
    }
}

using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace STrain.CQS.Test.Unit.Supports
{
    public static class HttpResponseExtensions
    {
        public static async Task<T?> ReadFromJsonAsync<T>(this HttpResponse response)
        {
            response.Body.Position = 0;
            return await JsonSerializer.DeserializeAsync<T>(response.Body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}

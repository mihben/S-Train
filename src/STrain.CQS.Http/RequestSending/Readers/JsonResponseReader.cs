using System.Net.Http.Json;
using System.Net.Mime;

namespace STrain.CQS.Http.RequestSending.Readers
{
    public class JsonResponseReader : IResponseReader
    {
        public async Task<object?> ReadAsync<T>(HttpResponseMessage message, CancellationToken cancellationToken)
        {
            if (message.Content.Headers.ContentType?.MediaType != System.Net.Mime.MediaTypeNames.Application.Json) throw new InvalidOperationException("Unsupported content type");

            return await message.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
        }
    }
}

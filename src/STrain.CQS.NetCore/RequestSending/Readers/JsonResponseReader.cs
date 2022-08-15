using STrain.CQS.NetCore.RequestSending.Parsers;
using System.Net.Http.Json;
using System.Net.Mime;

namespace STrain.CQS.NetCore.RequestSending.Readers
{
    public class JsonResponseReader : IResponseReader
    {
        public async Task<object?> ReadAsync<T>(HttpResponseMessage message, CancellationToken cancellationToken)
        {
            if (!message.Content.Headers.ContentType?.MediaType?.Equals(MediaTypeNames.Application.Json) ?? false) throw new InvalidOperationException("Invalid content type");

            return await message.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
        }
    }
}

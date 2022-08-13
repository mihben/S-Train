using STrain.CQS.NetCore.RequestSending.Parsers;
using System.Net.Http.Json;

namespace STrain.CQS.NetCore.RequestSending.Readers
{
    public class JsonResponseReader : IResponseReader
    {
        public async Task<object?> ReadAsync<T>(HttpResponseMessage message, CancellationToken cancellationToken)
        {
            return await message.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
        }
    }
}

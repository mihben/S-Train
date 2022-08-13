using STrain.CQS.NetCore.RequestSending.Parsers;

namespace STrain.CQS.NetCore.RequestSending.Readers
{
    public class StringResponseReader : IResponseReader
    {
        public async Task<object?> ReadAsync<T>(HttpResponseMessage message, CancellationToken cancellationToken)
        {
            return await message.Content.ReadAsStringAsync(cancellationToken);
        }
    }
}

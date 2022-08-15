﻿using STrain.CQS.NetCore.RequestSending.Parsers;
using System.Net.Mime;

namespace STrain.CQS.NetCore.RequestSending.Readers
{
    public class StringResponseReader : IResponseReader
    {
        public async Task<object?> ReadAsync<T>(HttpResponseMessage message, CancellationToken cancellationToken)
        {
            if (!message.Content.Headers.ContentType?.MediaType?.Equals(MediaTypeNames.Text.Plain, StringComparison.OrdinalIgnoreCase) ?? false) throw new InvalidOperationException("Invalid content type");

            return await message.Content.ReadAsStringAsync(cancellationToken);
        }
    }
}

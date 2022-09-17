﻿using System.Net.Mime;

namespace STrain.CQS.Http.RequestSending.Readers
{
    public class StringResponseReader : IResponseReader
    {
        public async Task<object?> ReadAsync<T>(HttpResponseMessage message, CancellationToken cancellationToken)
        {
            if (message.Content.Headers.ContentType?.MediaType != MediaTypeNames.Text.Plain) throw new InvalidOperationException("Unsupported content type");

            return await message.Content.ReadAsStringAsync(cancellationToken);
        }
    }
}
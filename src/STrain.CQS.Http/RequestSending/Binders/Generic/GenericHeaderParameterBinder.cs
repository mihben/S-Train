﻿using System.Net.Http.Headers;

namespace STrain.CQS.Http.RequestSending.Binders.Generic
{
    public class GenericHeaderParameterBinder : IHeaderParameterBinder
    {
        public Task BindAsync<TRequest>(TRequest request, HttpRequestHeaders headers, CancellationToken cancellationToken) where TRequest : IRequest
        {
            headers.Add("Request-Type", $"{request.GetType()}, {request.GetType().Assembly.GetName().Name}");

            return Task.CompletedTask;
        }
    }
}

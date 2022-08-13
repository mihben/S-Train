﻿using STrain.CQS.NetCore.RequestSending.Providers;
using System.Reflection;
using System.Web;

namespace STrain.CQS.NetCore.RequestSending.Attributive
{
    public class AttributiveQueryParameterProvider : IParameterProvider
    {
        public Task SetParametersAsync<TRequest>(HttpRequestMessage message, TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest
        {
            if (message.RequestUri is null) throw new ArgumentException("Uri is required");

            var type = typeof(TRequest);
            if (type.IsGenericRequest()) return Task.CompletedTask;

            if (type.GetCustomAttribute<QueryParameterAttribute>() is not null) message.RequestUri = message.RequestUri.SetQuery(type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty), request);
            else message.RequestUri = message.RequestUri.SetQuery(type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty).Where(p => p.GetCustomAttribute<QueryParameterAttribute>() is not null), request);
            return Task.CompletedTask;
        }
    }

    internal static class AttributiveQueryParameterProviderExtensions
    {
        public static Uri SetQuery<TRequest>(this Uri uri, IEnumerable<PropertyInfo> properties, TRequest request)
            where TRequest : IRequest
        {
            var uriBuilder = new UriBuilder(uri);
            var queryCollection = HttpUtility.ParseQueryString("");
            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<QueryParameterAttribute>();
                queryCollection.Add(attribute?.Name?.ToLower() ?? property.Name.ToLower(), property.GetValue(request)?.ToString() ?? string.Empty);
            }
            uriBuilder.Query = queryCollection.ToString();
            return uriBuilder.Uri;
        }
    }
}

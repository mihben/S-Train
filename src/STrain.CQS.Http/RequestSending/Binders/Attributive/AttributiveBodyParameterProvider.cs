using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Reflection;
using System.Text;

namespace STrain.CQS.Http.RequestSending.Providers.Attributive
{
    public class AttributiveBodyParameterProvider : IParameterProvider
    {
        private readonly ILogger<AttributiveBodyParameterProvider> _logger;

        public AttributiveBodyParameterProvider(ILogger<AttributiveBodyParameterProvider> logger)
        {
            _logger = logger;
        }

        public Task SetParametersAsync<TRequest>(HttpRequestMessage message, TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest
        {
            _logger.LogDebug("Setting HTTP request body");

            var type = typeof(TRequest);
            if (type.IsGenericRequest() || type.GetCustomAttribute<BodyParameterAttribute>() is not null)
            {
                message.Content = JsonContent.Create(request);
                DoneLogMessage();
                return Task.CompletedTask;
            }

            message.Content = new StringContent(JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                ContractResolver = new RequestContractResolver()
            }), Encoding.UTF8, MediaTypeNames.Application.Json);
            DoneLogMessage();
            return Task.CompletedTask;
        }

        private void DoneLogMessage()
        {
            _logger.LogDebug("Done setting HTTP request body");
        }
    }

    internal class RequestContractResolver : DefaultContractResolver
    {
        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            return objectType.GetMembers(Constants.HttpRequestSender.PropertyBindings).Where(m => m.GetCustomAttribute<BodyParameterAttribute>() is not null).ToList();
        }
    }
}

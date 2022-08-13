using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using STrain.CQS.Attributes.RequestSending.Http.Parameters;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Reflection;
using System.Text;

namespace STrain.CQS.NetCore.RequestSending.Providers.Attributive
{
    public class AttributiveBodyParameterProvider : IParameterProvider
    {
        public Task SetParametersAsync<TRequest>(HttpRequestMessage message, TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest
        {
            var type = typeof(TRequest);
            if (type.IsGenericRequest() || type.GetCustomAttribute<BodyParameterAttribute>() is not null)
            {
                message.Content = JsonContent.Create(request);
                return Task.CompletedTask;
            }

            message.Content = new StringContent(JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                ContractResolver = new RequestContractResolver()
            }), Encoding.UTF8, MediaTypeNames.Application.Json);
            return Task.CompletedTask;
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

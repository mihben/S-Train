using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using STrain.CQS.Attributes.RequestSending.Http.Parameters;
using System.Reflection;

namespace STrain.CQS.NetCore.RequestSending.Providers.Attributive
{
    public class AttributiveBodyParameterProvider : IParameterProvider
    {
        public Task SetParametersAsync<TRequest>(HttpRequestMessage message, TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest
        {
            var type = typeof(TRequest);
            var attribute = type.GetCustomAttribute<ParameterAttribute>();
            if (attribute is null|| attribute is BodyParameterAttribute)
            {
                message.Content = new StringContent(JsonConvert.SerializeObject(request, new JsonSerializerSettings
                {
                    ContractResolver = new RequestContractResolver()
                }));
            }

            return Task.CompletedTask;
        }
    }

    internal class RequestContractResolver : DefaultContractResolver
    {

        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            if (!objectType.IsAssignableTo(typeof(IRequest))) return base.GetSerializableMembers(objectType);
            return objectType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty).Where(m => m.GetCustomAttribute<BodyParameterAttribute>() is not null).ToList();
        }
    }
}

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Mime;
using System.Reflection;
using System.Text;

namespace STrain.CQS.Http.RequestSending.Binders.Attributive
{
    public class AttributiveBodyParameterBinder : IBodyParameterBinder
    {
        private readonly ILogger<AttributiveBodyParameterBinder> _logger;
        private readonly JsonSerializerSettings _serializerSettings = new()
        {
            ContractResolver = new AttributiveBodyParameterContractResolver()
        };

        public AttributiveBodyParameterBinder(ILogger<AttributiveBodyParameterBinder> logger)
        {
            _logger = logger;
        }

        public Task<HttpContent?> BindAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            _logger.LogDebug("Binding body parameter");

            var body = JsonConvert.SerializeObject(request, _serializerSettings);
            if (body.Equals("{}")) return Task.FromResult<HttpContent?>(null);

            return Task.FromResult((HttpContent?)new StringContent(body, Encoding.UTF8, MediaTypeNames.Application.Json));
        }
    }

    internal class AttributiveBodyParameterContractResolver : DefaultContractResolver
    {
        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            if (objectType.GetCustomAttribute<BodyParameterAttribute>() is not null) return base.GetSerializableMembers(objectType);

            return objectType.GetMembers(BindingFlags.Instance | BindingFlags.Public)
                                .Where(m => m.GetCustomAttribute<BodyParameterAttribute>() is not null).ToList();
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var result = base.CreateProperty(member, memberSerialization);
            var attribute = member.GetCustomAttribute<BodyParameterAttribute>();
            if (attribute?.Name is not null) result.PropertyName = attribute.Name;

            return result;
        }
    }
}

using STrain.CQS.Api;
using STrain.CQS.Attributes.RequestSending.Http;
using STrain.CQS.NetCore.RequestSending.Providers;
using System.Reflection;

namespace STrain.CQS.NetCore.RequestSending.Attributive
{
    public class AttributiveMethodProvider : IMethodProvider
    {
        public HttpMethod GetMethod<TRequest>()
            where TRequest : IRequest
        {
            var type = typeof(TRequest);
            if (type.IsGenericRequest())
            {
                if (type.IsAssignableTo(typeof(ICommand))) return HttpMethod.Post;
                if (type.IsAssignableTo(typeof(IQuery))) return HttpMethod.Get;
            }

            var attribute = type.GetCustomAttribute<MethodAttribute>();
            if (attribute is not null) return attribute.Method;

            throw new InvalidOperationException($"Method of {type} cannot be determined");
        }
    }
}

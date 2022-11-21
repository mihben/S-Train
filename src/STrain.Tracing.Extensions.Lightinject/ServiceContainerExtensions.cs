using STrain.CQS.Http.RequestSending.Binders;
using STrain.Tracing.Http.Binders;

namespace LightInject
{
    public static class ServiceContainerExtensions
    {
        public static void UseTracingHeaderParameterBinder(this ServiceContainer container, string key)
        {
            container.RegisterTransient<IHeaderParameterBinder, TracingHeaderParameterBinder>(key);
        }
    }
}
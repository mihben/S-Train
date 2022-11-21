using LightInject;

namespace STrain.CQS.NetCore.Builders
{
    public static class HttpRequesteSenderBuilderExtensions
    {
        public static HttpRequestSenderBuilder UseTracing(this HttpRequestSenderBuilder builder)
        {
            builder.Builder.Host.ConfigureContainer<ServiceContainer>((_, container) => container.UseTracingHeaderParameterBinder(builder.Key));
            return builder;
        }
    }
}

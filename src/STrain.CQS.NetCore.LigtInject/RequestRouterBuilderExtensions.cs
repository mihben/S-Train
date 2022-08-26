using LightInject;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using STrain.CQS.NetCore.Builders;
using STrain.CQS.NetCore.RequestSending;
using STrain.CQS.NetCore.RequestSending.Providers;

namespace STrain.CQS.NetCore.LigtInject
{
    public static class RequestRouterBuilderExtensions
    {
        public static HttpRequestSenderBuilder AddHttpSender(this RequestRouterBuilder builder, string key, Action<HttpRequestSenderOptions, IConfiguration> configure)
        {
            builder.Builder.Services.AddHttpRequestSender(key, configure);
            builder.Builder.Host.ConfigureContainer<IServiceRegistry>((_, registry) =>
            {
                registry.RegisterTransient<IRequestSender>(factory =>
                {
                    var clientFactory = factory.GetInstance<IHttpClientFactory>();

                    var pathProvider = factory.GetInstance<IPathProvider>(key);
                    var methodProvider = factory.GetInstance<IMethodProvider>(key);
                    var parameterProviders = new List<IParameterProvider>
                    {
                        factory.GetInstance<IParameterProvider>($"{key}.header"),
                        factory.GetInstance<IParameterProvider>($"{key}.query"),
                        factory.GetInstance<IParameterProvider>($"{key}.body")
                    };
                    var responseReaderProvider = factory.GetInstance<IResponseReaderProvider>(key);

                    return new HttpRequestSender(clientFactory.CreateClient(key), factory.GetInstance<IServiceProvider>(), pathProvider, methodProvider, parameterProviders, responseReaderProvider, factory.GetInstance<ILogger<HttpRequestSender>>());
                }, key);
            });
            return new HttpRequestSenderBuilder(key, builder.Builder);
        }
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using STrain.CQS.MVC.Constants;
using STrain.CQS.MVC.Options;
using STrain.CQS.NetCore.RequestSending;
using STrain.CQS.NetCore.RequestSending.Attributive;
using STrain.CQS.NetCore.RequestSending.Parsers;
using STrain.CQS.NetCore.RequestSending.Providers;
using STrain.CQS.NetCore.RequestSending.Providers.Attributive;
using STrain.CQS.NetCore.RequestSending.Readers;
using STrain.CQS.Senders;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;

namespace STrain.CQS.NetCore.Builders
{
    [ExcludeFromCodeCoverage]
    public class CQSBuilder
    {
        public WebApplicationBuilder Builder { get; }

        public CQSBuilder(WebApplicationBuilder builder)
        {
            Builder = builder;
        }

        public CQSBuilder AddPerformer<TPerformer, TImplementation>()
            where TPerformer : class
            where TImplementation : class, TPerformer
        {
            Builder.Services.AddPerformer<TPerformer, TImplementation>();
            return this;
        }

        public CQSBuilder AddGenericRequestHandler() => AddGenericRequestHandler("api");
        public CQSBuilder AddGenericRequestHandler(string path) => AddGenericRequestHandler((options, _) => options.Path = path);
        public CQSBuilder AddGenericRequestHandler(IConfigurationSection section) => AddGenericRequestHandler((options, _) => section.Bind(options));
        public CQSBuilder AddGenericRequestHandler(Action<GenericRequestHandlerOptions, IConfiguration> configure)
        {
            Builder.Services.AddGenericRequestHandler(configure);
            return this;
        }

        public CQSBuilder AddHttpSender(Action<HttpRequestSenderOptions, IConfiguration> configure)
        {
            Builder.Services.AddOptions<HttpRequestSenderOptions>()
                .Configure(configure);

            Builder.Services.AddTransient<IPathProvider, AttributivePathProvider>();
            Builder.Services.AddTransient<IMethodProvider, AttributiveMethodProvider>();
            Builder.Services.AddTransient<IParameterProvider, AttributiveQueryParameterProvider>();
            Builder.Services.AddTransient<IParameterProvider, AttributiveBodyParameterProvider>();
            Builder.Services.AddTransient<IParameterProvider, AttributiveHeaderParameterProvider>();
            Builder.Services.AddHttpClient<IRequestSender, HttpRequestSender>((provider, client) =>
            {
                var options = provider.GetRequiredService<IOptions<HttpRequestSenderOptions>>();
                client.BaseAddress = options.Value.BaseAddress;
            });

            Builder.Services.AddSingleton<IDictionary<string, Type>>(new Dictionary<string, Type>
            {
                [System.Net.Mime.MediaTypeNames.Text.Plain] = typeof(StringResponseReader),
                [System.Net.Mime.MediaTypeNames.Application.Json] = typeof(JsonResponseReader)
            });
            Builder.Services.AddTransient<StringResponseReader>();
            Builder.Services.AddTransient<JsonResponseReader>();

            return this;
        }
    }
}

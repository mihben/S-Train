using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using STrain.CQS.NetCore.RequestSending;
using STrain.CQS.NetCore.RequestSending.Attributive;
using STrain.CQS.NetCore.RequestSending.Providers;
using STrain.CQS.NetCore.RequestSending.Providers.Attributive;

namespace STrain.CQS.NetCore.Builders
{
    public class HttpRequestSenderBuilder
    {
        public WebApplicationBuilder Builder { get; }

        public HttpRequestSenderBuilder(WebApplicationBuilder builder)
        {
            Builder = builder;
        }

        public HttpRequestSenderBuilder UseDefaults()
        {
            UseAttributivePathProvider();
            UseAttributeMethodProvider();
            UseAttributiveParameterProviders();
            UseResponseReaders();

            return this;
        }

        public HttpRequestSenderBuilder UseAttributivePathProvider() => UsePathProvider<AttributivePathProvider>();
        public HttpRequestSenderBuilder UsePathProvider<TPathProvider>()
            where TPathProvider : class, IPathProvider
        {
            Builder.Services.AddPathProvider<TPathProvider>();
            return this;
        }

        public HttpRequestSenderBuilder UseAttributeMethodProvider() => UseMethodProvider<AttributiveMethodProvider>();
        public HttpRequestSenderBuilder UseMethodProvider<TMethodProvider>()
            where TMethodProvider : class, IMethodProvider
        {
            Builder.Services.AddMethodProvider<TMethodProvider>();
            return this;
        }

        public HttpRequestSenderBuilder UseAttributiveParameterProviders() => UseParameterProviders<AttributiveHeaderParameterProvider, AttributiveQueryParameterProvider, AttributiveBodyParameterProvider>();
        public HttpRequestSenderBuilder UseParameterProviders<THeaderParameterProvider, TQueryParameterProvider, TBodyParameterProvider>()
            where THeaderParameterProvider : class, IParameterProvider
            where TQueryParameterProvider : class, IParameterProvider
            where TBodyParameterProvider : class, IParameterProvider
        {
            Builder.Services.AddParameterProvider<THeaderParameterProvider>();
            Builder.Services.AddParameterProvider<TQueryParameterProvider>();
            Builder.Services.AddParameterProvider<TBodyParameterProvider>();
            return this;
        }

        public HttpRequestSenderBuilder UseResponseReaders() => UseResponseReaders(register => register.UseDefaults());
        public HttpRequestSenderBuilder UseResponseReaders(Action<ResponseReadersRegister> registrate)
        {
            var registry = new ResponseReaderRegistry();
            registrate(new ResponseReadersRegister(registry, Builder));
            Builder.Services.AddResponseReaderRegistry(registry);
            return this;
        }
    }
}

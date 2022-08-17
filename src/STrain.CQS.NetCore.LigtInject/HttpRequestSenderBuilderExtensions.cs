using STrain.CQS.NetCore.Builders;
using STrain.CQS.NetCore.RequestSending.Attributive;
using STrain.CQS.NetCore.RequestSending.Providers.Attributive;
using STrain.CQS.NetCore.RequestSending.Providers;
using STrain.CQS.NetCore.RequestSending;
using System.Runtime.CompilerServices;
using LightInject;
using Microsoft.Extensions.DependencyInjection;

namespace STrain.CQS.NetCore.LigtInject
{
    public static class HttpRequestSenderBuilderExtensions
    {
        public HttpRequestSenderBuilder UseDefaults(this HttpRequestSenderBuilder builder)
        {
            UseAttributivePathProvider();
            UseAttributeMethodProvider();
            UseAttributiveParameterProviders();
            UseResponseReaders();

            return builder;
        }

        public static HttpRequestSenderBuilder UseAttributivePathProvider(this HttpRequestSenderBuilder builder) => builder.UsePathProvider<AttributivePathProvider>();
        public static HttpRequestSenderBuilder UsePathProvider<TPathProvider>(this HttpRequestSenderBuilder builder)
            where TPathProvider : class, IPathProvider
        {
            builder.Builder.Host.ConfigureContainer<IServiceRegistry>((_, registry) => registry.RegisterTransient<IPathProvider, TPathProvider>(builder.Key));
            return builder;
        }

        public static HttpRequestSenderBuilder UseAttributeMethodProvider(this HttpRequestSenderBuilder builder) => builder.UseMethodProvider<AttributiveMethodProvider>();
        public static HttpRequestSenderBuilder UseMethodProvider<TMethodProvider>(this HttpRequestSenderBuilder builder)
            where TMethodProvider : class, IMethodProvider
        {
            builder.Builder.Host.ConfigureContainer<IServiceRegistry>((_, registry) => registry.RegisterScoped<IMethodProvider, TMethodProvider>(builder.Key));
            return builder;
        }

        public static HttpRequestSenderBuilder UseAttributiveParameterProviders(this HttpRequestSenderBuilder builder) => builder.UseParameterProviders<AttributiveHeaderParameterProvider, AttributiveQueryParameterProvider, AttributiveBodyParameterProvider>();
        public static HttpRequestSenderBuilder UseParameterProviders<THeaderParameterProvider, TQueryParameterProvider, TBodyParameterProvider>(this HttpRequestSenderBuilder builder)
            where THeaderParameterProvider : class, IParameterProvider
            where TQueryParameterProvider : class, IParameterProvider
            where TBodyParameterProvider : class, IParameterProvider
        {
            builder.Builder.Host.ConfigureContainer<IServiceRegistry>((_, registry) =>
            {
                registry.RegisterTransient<IParameterProvider, THeaderParameterProvider>(builder.Key);
                registry.RegisterTransient<IParameterProvider, TQueryParameterProvider>(builder.Key);
                registry.RegisterTransient<IParameterProvider, TBodyParameterProvider>(builder.Key);
            });
            return builder;
        }

        public static HttpRequestSenderBuilder UseResponseReaders(this HttpRequestSenderBuilder builder) => builder.UseResponseReaders(register => register.UseDefaults());
        public static HttpRequestSenderBuilder UseResponseReaders(this HttpRequestSenderBuilder builder, Action<ResponseReadersRegister> registrate)
        {
            var registry = new ResponseReaderRegistry();
            registrate(new ResponseReadersRegister(registry, builder.Builder));
            builder.Builder.Host.ConfigureContainer<IServiceRegistry>((_, registry) =>
            {
                registry.RegisterInstance<ResponseReadersRegister>(registry, builder.Key);
            });
            return builder;
        }
    }
}

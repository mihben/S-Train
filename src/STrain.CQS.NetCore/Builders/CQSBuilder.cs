using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using STrain.CQS.MVC.Options;
using STrain.CQS.NetCore.RequestSending;
using STrain.CQS.Receivers;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

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

        public MvcRequestReceiverBuilder AddMvcRequestReceiver()
        {
            Builder.Services.AddTransient<IMvcRequestReceiver, MvcRequestReceiver>();
            return new MvcRequestReceiverBuilder(Builder);
        }
    }
}

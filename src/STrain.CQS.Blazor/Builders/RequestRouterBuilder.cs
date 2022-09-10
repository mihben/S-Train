using LightInject;
using LightInject.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace STrain.CQS.Blazor.Builders
{
    public class RequestRouterBuilder
    {
        public WebAssemblyHostBuilder Builder { get; }

        public RequestRouterBuilder(WebAssemblyHostBuilder builder)
        {
            Builder = builder;
        }

        public RequestRouterBuilder AddHttpSender(string key)
        {
            Builder.ConfigureContainer<IServiceRegistry>(new LightInjectServiceProviderFactory(ContainerOptions.Default), registry => registry.AddHttpSender(key));
        }
    }
}

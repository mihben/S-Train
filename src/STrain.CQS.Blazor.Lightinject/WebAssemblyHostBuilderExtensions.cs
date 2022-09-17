using LightInject;
using LightInject.Microsoft.DependencyInjection;
using STrain.CQS.Blazor.Lightinject;

namespace Microsoft.AspNetCore.Components.WebAssembly.Hosting
{
    public static class WebAssemblyHostBuilderExtensions
    {
        public static void UseLightinject(this WebAssemblyHostBuilder builder)
        {
            var container = new ServiceContainer(ContainerOptions.Default.Clone().WithMicrosoftSettings());
            ServiceProviderFactoryStore.Factory = new LightInjectServiceProviderFactory(container);
        }

        public static void ConfigureContainer(this WebAssemblyHostBuilder builder, Action<IServiceContainer> container)
            => builder.ConfigureContainer(ServiceProviderFactoryStore.Factory, container);
    }
}

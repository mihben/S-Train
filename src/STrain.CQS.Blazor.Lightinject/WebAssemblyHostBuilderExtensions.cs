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
            ContainerContext.Container = container;
            builder.ConfigureContainer(new LightInjectServiceProviderFactory(ContainerContext.Container));
        }

        public static void ConfigureContainer(this WebAssemblyHostBuilder builder, Action<IServiceContainer> container)
            => container(ContainerContext.Container);
    }
}

using LightInject;
using LightInject.Microsoft.DependencyInjection;
using STrain.CQS.Blazor.LightInject;

namespace Microsoft.AspNetCore.Components.WebAssembly.Hosting
{
    public static class WebAssemblyHostBuilderExtensions
    {
        public static void UseLightinject(this WebAssemblyHostBuilder builder)
        {
            ContainerContext.Container = new ServiceContainer(ContainerOptions.Default.Clone().WithMicrosoftSettings());
            builder.ConfigureContainer(new LightInjectServiceProviderFactory(ContainerContext.Container));
        }

        public static void ConfigureContainer(this WebAssemblyHostBuilder builder, Action<IServiceContainer> container)
        {
            if (ContainerContext.Container is null) throw new InvalidOperationException("Container has not been initialized. Use WebAssemblyHostBuilder.UseLightinject() method.");

            container(ContainerContext.Container);
        }
    }
}

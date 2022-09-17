using LightInject;
using Microsoft.Extensions.DependencyInjection;

namespace STrain.CQS.Blazor.Lightinject
{
    public static class ServiceProviderFactoryStore
    {
        public static IServiceProviderFactory<IServiceContainer>? Factory { get; internal set; }
    }
}

using LightInject;
using LightInject.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace STrain.CQS.Test.Function.Workarounds
{
    internal class LightinjectWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseServiceProviderFactory(new CustomServiceProviderFactory());
            return base.CreateHost(builder);
        }
    }

    internal class CustomServiceProviderFactory : IServiceProviderFactory<IServiceContainer>
    {
        private IServiceCollection? _services;

        public IServiceContainer CreateBuilder(IServiceCollection services)
        {
            _services = services;
            return new CustomServiceContainer(services).CustomBuild();
        }

        public IServiceProvider CreateServiceProvider(IServiceContainer containerBuilder)
        {
            return containerBuilder.CreateServiceProvider(_services);
        }
    }

    internal class CustomServiceContainer : ServiceContainer
    {
        private readonly IServiceCollection _services;

        public CustomServiceContainer(IServiceCollection services)
            : base(ContainerOptions.Default.WithMicrosoftSettings())
        {
            _services = services;
        }

        public IServiceContainer CustomBuild()
        {
            var serviceProvider = _services.CreateLightInjectServiceProvider();
#pragma warning disable CS0612 // Type or member is obsolete
            var filters = serviceProvider.GetRequiredService<IEnumerable<IStartupConfigureContainerFilter<IServiceContainer>>>();
#pragma warning restore CS0612 // Type or member is obsolete

            foreach (var filter in filters)
            {
                filter.ConfigureContainer(_ => { })(this);
            }

            return this;
        }
    }
}

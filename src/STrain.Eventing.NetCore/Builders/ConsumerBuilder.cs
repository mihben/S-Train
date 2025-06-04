using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using STrain.Eventing.Consumers;
using STrain.Eventing.NetCore.Initializers;

namespace STrain.Eventing.NetCore.Builders
{
	public class ConsumerBuilder
	{
		public WebApplicationBuilder Builder { get; }
		public string? Key { get; }

		public ConsumerBuilder(WebApplicationBuilder builder, string? key)
		{
			Builder = builder;
			Key = key;
		}

		public ConsumerBuilder AutoStateManagement()
		{
			Builder.Services.AddHostedService(provider =>
			{
				if (Key is null) return new ConsumerStateManager(provider.GetRequiredService<IConsumer>());
				else return new ConsumerStateManager(provider.GetRequiredKeyedService<IConsumer>(Key));
			});

			return this;
		}
	}
}

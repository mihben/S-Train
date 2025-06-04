using KurrentDB.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using STrain.Eventing.Consumers;
using STrain.Eventing.Dispatchers;
using STrain.Eventing.KurrentDB.Consumers;
using STrain.Eventing.KurrentDB.Options;
using STrain.Eventing.KurrentDB.Unwrappers;
using STrain.Eventing.NetCore.Builders;

namespace STrain.Eventing.KurrentDB.NetCore.Builders
{
	public class KurrentDBBuilder
	{
		public WebApplicationBuilder Builder { get; }

		public KurrentDBBuilder(WebApplicationBuilder builder)
		{
			Builder = builder;
		}

		public ConsumerBuilder Consume(string stream)
		{
			Builder.Services.AddOptions<ConsumerOptions>(stream)
				.Configure(options => options.Stream = stream)
				.ValidateDataAnnotations()
				.ValidateOnStart();

			Builder.Services.AddKeyedSingleton<IConsumer>(stream, (provider, key) => new Consumer(provider.GetRequiredService<KurrentDBClient>(),
																							provider.GetRequiredService<IOptionsSnapshot<ConsumerOptions>>().Get(stream),
																							provider.GetRequiredKeyedService<IUnwrapper>(stream),
																							provider.GetRequiredService<IEventDispatcher>(),
																							provider.GetRequiredService<ILogger<Consumer>>())
			);

			return new ConsumerBuilder(Builder, stream);
		}
	}
}

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using STrain.Eventing.Publishers;
using STrain.Eventing.RabbitMQ.Options;
using STrain.Eventing.RabbitMQ.Publishers;

namespace STrain.Eventing.RabbitMQ.NetCore.Builders
{
	public class RabbitMQConnectionBuilder
	{
		public WebApplicationBuilder Builder { get; }
		public string? Key { get; }

		public RabbitMQConnectionBuilder(WebApplicationBuilder builder, string? key)
		{
			Builder = builder;
			Key = key;
		}

		public RabbitMQConnectionBuilder AddListener(Action<RabbitMQListenerBuilder> build)
		{
			if (Key is null)
			{
				//Builder.Services.AddHostedService(provider => new ListenerInitializer(provider.GetRequiredService<IListener>()));
				//Builder.Services.AddSingleton((provider) => provider.GetRequiredService<IConnection>().CreateChannelAsync().GetAwaiter().GetResult());
				//Builder.Services.AddTransient<IListener, Consumer>();
			}
			else
			{
				//Builder.Services.AddHostedService(provider => new ListenerInitializer(provider.GetRequiredKeyedService<IListener>(Key)));
				//Builder.Services.AddKeyedSingleton(Key, (provider, key) => provider.GetRequiredKeyedService<IConnection>(key).CreateChannelAsync().GetAwaiter().GetResult());
				//Builder.Services.AddKeyedTransient<IListener>(Key, (provider, key) => new Consumer(provider.GetRequiredService<IOptions<RabbitMQOptions>>(), provider.GetRequiredKeyedService<IChannel>(key), provider.GetKeyedServices<IReceiver>(key), provider.GetRequiredService<ILogger<Consumer>>()));
			}

			build(new RabbitMQListenerBuilder(Builder, Key));

			return this;
		}

		public RabbitMQConnectionBuilder AddPublisher(string key)
		{
			if (Key is null)
			{
				Builder.Services.AddSingleton((provider) => provider.GetRequiredService<IConnection>().CreateChannelAsync().GetAwaiter().GetResult());
				Builder.Services.AddTransient<IEventPublisher, RabbitMQEventPublisher>();
			}
			else
			{
				Builder.Services.AddKeyedSingleton(Key, (provider, key) => provider.GetRequiredKeyedService<IConnection>(key).CreateChannelAsync().GetAwaiter().GetResult());
				Builder.Services.AddKeyedTransient<IEventPublisher>(key, (provider, _) =>
				{
					return new RabbitMQEventPublisher(provider.GetRequiredKeyedService<IChannel>(Key), provider.GetRequiredService<IOptions<RabbitMQOptions>>(), provider.GetRequiredService<ILogger<RabbitMQEventPublisher>>());
				});
			}

			return this;
		}
	}
}

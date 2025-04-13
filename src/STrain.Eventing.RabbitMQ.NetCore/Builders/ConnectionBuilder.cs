using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using STrain.Eventing.Publishers;
using STrain.Eventing.RabbitMQ.Options;
using STrain.Eventing.RabbitMQ.Publishers;

namespace STrain.Eventing.RabbitMQ.NetCore.Builders
{
	public class ConnectionBuilder
	{
		public WebApplicationBuilder Builder { get; }
		public string? Key { get; }

		public ConnectionBuilder(WebApplicationBuilder builder, string? key)
		{
			Builder = builder;
			Key = key;
		}

		public ConnectionBuilder AddConsumer(string section)
		{
			return AddConsumer(builder => builder.AddGenericUnwrapper().AutoStateManagement(), (options, configuration) => configuration.Bind(section, options));
		}

		public ConnectionBuilder AddConsumer(Action<ConsumerBuilder> build, Action<ConsumerOptions, IConfiguration> configure)
		{
			Builder.Services.AddOptions<ConsumerOptions>(Key)
				.Configure(configure)
				.ValidateDataAnnotations()
				.ValidateOnStart();

			if (Key is null) Builder.Services.AddConsumer();
			else Builder.Services.AddConsumer(Key);

			build(new ConsumerBuilder(Builder, Key));

			return this;
		}

		public ConnectionBuilder AddPublisher(string key)
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

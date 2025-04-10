using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.AMQP.Client.Impl;
using STrain.Eventing.Listeners;
using STrain.Eventing.NetCore.Builders;
using STrain.Eventing.RabbitMQ.Listeners;
using STrain.Eventing.RabbitMQ.NetCore.Builders;
using STrain.Eventing.RabbitMQ.Options;

namespace STrain.Eventing.RabbitMQ.NetCore.Extensions
{
	public static class ListenerBuilderExtensions
	{
		public static ListenerBuilder AddRabbitMQListener(this ListenerBuilder builder, Action<RabbitMQOptions, IConfiguration> configure, Action<RabbitMQListenerBuilder> build)
		{
			builder.Builder.Services.AddOptions<RabbitMQOptions>()
				.Configure(configure)
				.ValidateDataAnnotations()
				.ValidateOnStart();

			builder.Builder.Services.AddTransient<IListener, RabbitMQListener>();
			builder.Builder.Services.AddSingleton((provider) =>
			{
				var options = provider.GetRequiredService<IOptions<RabbitMQOptions>>();
				return AmqpEnvironment.Create(ConnectionSettingBuilder
					.Create()
					.Host(options.Value.Host)
					.Port(options.Value.Port)
					.User(options.Value.User)
					.Password(options.Value.Password)
					.Build());
			});

			build(new RabbitMQListenerBuilder(builder.Builder));

			return builder;
		}
	}
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using STrain.Eventing.NetCore.Builders;
using STrain.Eventing.RabbitMQ.NetCore.Builders;
using STrain.Eventing.RabbitMQ.Options;

namespace STrain.Eventing.RabbitMQ.NetCore.Extensions
{
	public static class EventingBuilderExtensions
	{
		public static RabbitMQBuilder AddRabbitMQ(this EventingBuilder builder, Action<RabbitMQOptions, IConfiguration> configure)
		{
			builder.Builder.Services.AddOptions<RabbitMQOptions>()
				.Configure(configure)
				.ValidateDataAnnotations()
				.ValidateOnStart();

			return new RabbitMQBuilder(builder.Builder);
		}
	}
}

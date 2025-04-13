using STrain.Eventing.NetCore.Builders;
using STrain.Eventing.NetCore.LightInject;
using STrain.Eventing.RabbitMQ.NetCore.Extensions;
using STrain.Sample.Api;
using STrain.Sample.Backend.Performers;

namespace STrain.Sample.Backend.Wireup
{
	public static class EventingWireup
	{
		public static void Build(EventingBuilder builder)
		{
			builder.AddHandler<SampleEvent, SampleEventHandler>();
			builder.AddEventHandlerLogger();

			builder.AddPublisher(_ => "rabbitmq");

			var rabbitmq = builder.AddRabbitMQ((options, configuration) => configuration.Bind("RabbitMQ", options))
				.AddConnection()
					.AddConsumer("RabbitMQ:Consumer");

		}
	}
}

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

			builder.AddRouter(_ => "test-exchange");

			var rabbitmq = builder.AddRabbitMQ((options, configuration) => configuration.Bind("RabbitMQ", options))
				.AddConnection()
					.AddConsumer("RabbitMQ:Consumer")
					.AddPublisher("test-exchange", "RabbitMQ:Publisher");

		}
	}
}

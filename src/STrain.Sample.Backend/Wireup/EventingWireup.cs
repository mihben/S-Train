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

			var rabbitMqBuilder = builder.AddRabbitMQ((options, configuration) => configuration.Bind("RabbitMQ", options));
			rabbitMqBuilder.AddConnection("receive").AddListener(builder => builder.AddDefaultReceiver());
			rabbitMqBuilder.AddConnection("publish").AddPublisher("rabbitmq");

		}
	}
}

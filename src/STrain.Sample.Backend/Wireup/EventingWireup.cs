using STrain.Eventing.NetCore.Builders;
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

			builder.AddListener()
				.AddRabbitMQListener((options, configuration) => configuration.Bind("RabbitMQ", options), builder => builder.AddDefaultReceiver());
		}
	}
}

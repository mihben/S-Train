using STrain.Eventing.KurrentDB.NetCore.Extensions;
using STrain.Eventing.NetCore.Builders;
using STrain.Eventing.NetCore.LightInject;
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

			builder.AddRouter(_ => "kurrentdb");

			builder.AddKurrentDB((settings, configuration) => configuration.Bind("KurrentDB", settings))
				.Consume("sample-events")
					.AddGenericUnwrapper()
					.AutoStateManagement();
		}
	}
}

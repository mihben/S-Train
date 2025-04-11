using LightInject;
using STrain.Eventing.Behaviors;
using STrain.Eventing.Handlers;
using STrain.Eventing.NetCore.Builders;

namespace STrain.Eventing.NetCore.LightInject
{
	public static class EventingBuilderExtension
	{
		public static EventingBuilder AddEventHandlerLogger(this EventingBuilder builder)
		{
			builder.Builder.Host.ConfigureContainer<IServiceRegistry>((_, registry) => registry.Decorate(typeof(IEventHandler<>), typeof(EventHandlerLogger<>)));

			return builder;
		}
	}
}

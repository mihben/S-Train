using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using STrain.Eventing.Api;
using STrain.Eventing.Handlers;

namespace STrain.Eventing.NetCore.Builders
{
	public class EventingBuilder
	{
		public readonly WebApplicationBuilder Builder;

		public EventingBuilder(WebApplicationBuilder builder)
		{
			Builder = builder;
		}

		public void AddHandler<TEvent, TImplementation>()
			where TEvent : IEvent
			where TImplementation : class, IEventHandler<TEvent>
		{
			Builder.Services.AddHandler<TEvent, TImplementation>();
		}

		public PublisherBuilder AddPublisher(Func<IEvent, string> routing)
		{
			Builder.Services.AddEventPublisher(routing);

			return new PublisherBuilder(Builder);
		}
	}
}

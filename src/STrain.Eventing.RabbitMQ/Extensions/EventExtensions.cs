using STrain.Eventing.Api;

namespace STrain.Eventing.RabbitMQ.Extensions
{
	public static class EventExtensions
	{
		public static string GetEventType<TEvent>(this TEvent @event)
			where TEvent : IEvent
		{
			return $"{@event.GetType().FullName}, {@event.GetType().Assembly.GetName().Name}";
		}
	}
}

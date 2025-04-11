using STrain.Eventing.Api;

namespace STrain
{
	public static class EventExtensions
	{
		public static string LogEntry(this IEvent @event)
		{
			return $"{@event.GetType()}";
		}
	}
}

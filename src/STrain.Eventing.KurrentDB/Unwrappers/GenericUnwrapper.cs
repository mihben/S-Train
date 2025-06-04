using KurrentDB.Client;
using STrain.Eventing.Api;
using System.Text.Json;

namespace STrain.Eventing.KurrentDB.Unwrappers
{
	public class GenericUnwrapper : IUnwrapper
	{
		public async Task<IEvent> UnwrapAsync(ResolvedEvent @event, CancellationToken cancellationToken)
		{
			var type = Type.GetType(@event.Event.EventType);
			await using var stream = new MemoryStream(@event.Event.Data.ToArray());
			return (await JsonSerializer.DeserializeAsync(stream, returnType: type, cancellationToken: cancellationToken)) as IEvent;
		}
	}
}

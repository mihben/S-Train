using KurrentDB.Client;
using STrain.Eventing.Api;

namespace STrain.Eventing.KurrentDB.Unwrappers
{
	public interface IUnwrapper
	{
		Task<IEvent> UnwrapAsync(ResolvedEvent @event, CancellationToken cancellationToken);
	}
}

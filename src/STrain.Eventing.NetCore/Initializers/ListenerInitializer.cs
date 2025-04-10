using Microsoft.Extensions.Hosting;
using STrain.Eventing.Listeners;

namespace STrain.Eventing.NetCore.Initializers
{
	public class ListenerInitializer : BackgroundService
	{
		private readonly IEnumerable<IListener> _listeners;

		public ListenerInitializer(IEnumerable<IListener> listeners)
		{
			_listeners = listeners;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			await Parallel.ForEachAsync(_listeners, async (l, ct) => await l.ListenAsync(ct));
		}
	}
}

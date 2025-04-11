using Microsoft.Extensions.Hosting;
using STrain.Eventing.Listeners;

namespace STrain.Eventing.NetCore.Initializers
{
	public class ListenerInitializer : BackgroundService
	{
		private readonly IListener _listener;

		public ListenerInitializer(IListener listener)
		{
			_listener = listener;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			await _listener.ListenAsync(stoppingToken);
		}
	}
}

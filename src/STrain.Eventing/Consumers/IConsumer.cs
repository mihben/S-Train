namespace STrain.Eventing.Consumers
{
	public interface IConsumer
	{
		Task InitializeAsync(CancellationToken cancellationToken);
		Task StartAsync(CancellationToken cancellationToken);
		Task StopAsync(CancellationToken cancellationToken);
	}
}

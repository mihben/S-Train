namespace STrain.Eventing.Consumers
{
	public interface IConsumer
	{
		Task StartAsync(CancellationToken cancellationToken);
		Task StopAsync(CancellationToken cancellationToken);
	}
}

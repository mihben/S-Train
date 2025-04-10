namespace STrain.Eventing.Listeners
{
	public interface IListener : IAsyncDisposable
	{
		Task ListenAsync(CancellationToken cancellationToken);
	}
}

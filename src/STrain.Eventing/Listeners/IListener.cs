namespace STrain.Eventing.Listeners
{
	public interface IListener
	{
		Task ListenAsync(CancellationToken cancellationToken);
	}
}

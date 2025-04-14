namespace STrain.Eventing.Consumers
{
    public interface IConsumer : IInitializer
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}

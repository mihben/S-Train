namespace STrain.Eventing
{
    public interface IInitializer
    {
        Task InitializeAsync(CancellationToken cancellationToken);
    }
}

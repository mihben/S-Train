using STrain.Sample.Api;

namespace STrain.Sample.Backend.Performers
{
    public class AuthorizationPerformer : ICommandPerformer<AuthorizedCommand>,
                                        ICommandPerformer<UnathorizedCommand>,
                                        ICommandPerformer<AllowAnonymusCommand>
    {
        public Task PerformAsync(AuthorizedCommand command, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task PerformAsync(UnathorizedCommand command, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task PerformAsync(AllowAnonymusCommand command, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

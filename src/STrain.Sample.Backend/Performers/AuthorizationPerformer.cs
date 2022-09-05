using STrain.Sample.Api;

namespace STrain.Sample.Backend.Performers
{
    public class AuthorizationPerformer : ICommandPerformer<Authorization.AuthorizedCommand>,
                                        ICommandPerformer<Authorization.AllowAnonymusCommand>
    {
        public Task PerformAsync(Authorization.AuthorizedCommand command, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task PerformAsync(Authorization.AllowAnonymusCommand command, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

namespace STrain.CQS.Validations
{
    public class QueryValidator : IQueryDispatcher
    {
        private readonly IRequestValidator _validator;
        private readonly IQueryDispatcher _dispatcher;

        public QueryValidator(IRequestValidator validator, IQueryDispatcher dispatcher)
        {
            _validator = validator;
            _dispatcher = dispatcher;
        }

        public async Task<T> DispatchAsync<T>(Query<T> query, CancellationToken cancellationToken)
        {
            await _validator.ValidateAsync(query, cancellationToken);
            return await _dispatcher.DispatchAsync(query, cancellationToken);
        }
    }
}

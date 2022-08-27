namespace STrain.CQS.Validations
{
    public interface IRequestValidator
    {
        Task ValidateAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest;
    }
}

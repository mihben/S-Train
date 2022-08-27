using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using STrain.CQS.Validations;
using ValidationException = STrain.Core.Exceptions.ValidationException;

namespace STrain.CQS.NetCore.Validation
{
    public class FluentRequestValidator : IRequestValidator
    {
        private readonly IServiceProvider _serviceProvider;

        public FluentRequestValidator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ValidateAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest
        {
            var validator = _serviceProvider.GetService<IValidator<TRequest>>();
            if (validator is null) return;

            var validationResult = await validator.ValidateAsync(new ValidationContext<TRequest>(request), cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors.ToDictionary(e => e.PropertyName, e => e.ErrorMessage));
            }
        }
    }
}

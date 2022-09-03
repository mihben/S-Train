using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using STrain.CQS.Validations;
using ValidationException = STrain.Core.Exceptions.ValidationException;

namespace STrain.CQS.NetCore.Validation
{
    public class FluentRequestValidator : IRequestValidator
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<FluentRequestValidator> _logger;

        public FluentRequestValidator(IServiceProvider serviceProvider, ILogger<FluentRequestValidator> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task ValidateAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest
        {
            var validator = _serviceProvider.GetService<IValidator<TRequest>>();
            if (validator is null)
            {
                _logger.LogDebug("Validator not found");
                return;
            }

            var validationResult = await validator.ValidateAsync(new ValidationContext<TRequest>(request), cancellationToken);
            _logger.LogTrace("Validation result: {@ValidationResult}", validationResult);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors.ToDictionary(e => e.PropertyName, e => e.ErrorMessage));
            }
        }
    }
}

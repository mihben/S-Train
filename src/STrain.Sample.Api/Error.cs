using FluentValidation;

namespace STrain.Sample.Api
{
    public static class Error
    {
        public record ValidatedCommand : Command
        {
            public string Value { get; }

            public ValidatedCommand(string value)
            {
                Value = value;
            }
        }

        public class ValidatedCommandValidator : AbstractValidator<ValidatedCommand>
        {
            public ValidatedCommandValidator()
            {
                RuleFor(c => c.Value)
                    .NotEmpty();
            }
        }
    }
}

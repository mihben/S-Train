using FluentValidation;

namespace STrain.Sample.Api
{
    public record SampleCommand : Command
    {
        public string Parameter { get; }

        public SampleCommand(string parameter)
        {
            Parameter = parameter;
        }
    }

    public class SampleCommandValidator : AbstractValidator<SampleCommand>
    {
        public SampleCommandValidator()
        {
            RuleFor(c => c.Parameter)
                .NotEmpty();
        }
    }
}
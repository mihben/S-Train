using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using STrain.CQS.NetCore.Validation;
using STrain.CQS.Validations;

namespace STrain.CQS.NetCore.Builders
{
    public class RequestValidatorBuilder
    {
        public WebApplicationBuilder Builder { get; }

        public RequestValidatorBuilder(WebApplicationBuilder builder)
        {
            Builder = builder;
        }

        public RequestValidatorBuilder UseRequestValidator<TRequestValidator>()
            where TRequestValidator : class, IRequestValidator
        {
            Builder.Services.AddRequestValidator<TRequestValidator>();
            return this;
        }

        public RequestValidatorBuilder UseFluentRequestValidator(Action<FluentValidatorBuilder> build)
        {
            build(new FluentValidatorBuilder(Builder));
            return UseRequestValidator<FluentRequestValidator>();
        }
    }
}

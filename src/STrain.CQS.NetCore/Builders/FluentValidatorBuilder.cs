using FluentValidation;
using Microsoft.AspNetCore.Builder;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace STrain.CQS.NetCore.Builders
{
    [ExcludeFromCodeCoverage]
    public class FluentValidatorBuilder
    {
        public WebApplicationBuilder Builder { get; }

        public FluentValidatorBuilder(WebApplicationBuilder builder)
        {
            Builder = builder;
        }

        public FluentValidatorBuilder RegistrateFrom(Assembly assembly)
        {
            Builder.Services.AddValidatorsFromAssembly(assembly);
            return this;
        }

        public FluentValidatorBuilder RegistrateFrom<T>()
        {
            Builder.Services.AddValidatorsFromAssemblyContaining<T>();
            return this;
        }
    }
}

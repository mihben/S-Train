using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace STrain.CQS.NetCore.Builders
{
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

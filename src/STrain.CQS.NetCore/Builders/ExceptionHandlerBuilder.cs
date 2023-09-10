using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using STrain.CQS.NetCore.ExceptionHandling;

namespace STrain.CQS.NetCore.Builders
{
    public class ExceptionHandlerBuilder
    {
        public IServiceCollection Services { get; }

        public ExceptionHandlerBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public ExceptionHandlerBuilder UseWriter<TProblemDetailsWriter>()
            where TProblemDetailsWriter : class, IProblemDetailsWriter
        {
            Services.AddTransient<IProblemDetailsWriter, TProblemDetailsWriter>();

            return this;
        }

        public ExceptionHandlerBuilder UseExceptionWriter() => UseWriter<ExceptionProblemDetailsWriter>();
        public ExceptionHandlerBuilder UseNotFoundExceptionWriter() => UseWriter<NotFoundExceptionProblemDetailsWriter>();
        public ExceptionHandlerBuilder UseVerificationExceptionWriter() => UseWriter<VerificationExceptionProblemDetailsWriter>();
        public ExceptionHandlerBuilder UseValidationExceptionWriter() => UseWriter<ValidationExceptionProblemDetailsWriter>();

        public ExceptionHandlerBuilder UseDefaultWriters() => UseNotFoundExceptionWriter()
            .UseVerificationExceptionWriter()
            .UseValidationExceptionWriter()
            .UseExceptionWriter();
    }
}

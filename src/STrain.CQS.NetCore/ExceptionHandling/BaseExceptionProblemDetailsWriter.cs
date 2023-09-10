using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using STrain.CQS.Http.RequestSending;

namespace STrain.CQS.NetCore.ExceptionHandling
{
    public abstract class BaseExceptionProblemDetailsWriter<TException> : IProblemDetailsWriter
        where TException : Exception
    {
        protected readonly ILogger<BaseExceptionProblemDetailsWriter<TException>> _logger;

        protected BaseExceptionProblemDetailsWriter(ILogger<BaseExceptionProblemDetailsWriter<TException>> logger)
        {
            _logger = logger;
        }

        public bool CanWrite(ProblemDetailsContext context) => context.HttpContext.Features.GetRequiredFeature<IExceptionHandlerPathFeature>().Error is TException;

        public async ValueTask WriteAsync(ProblemDetailsContext context)
        {
            _logger.LogDebug("Attempting to serialize problem details");
            var feature = context.HttpContext.Features.GetRequiredFeature<IExceptionHandlerPathFeature>();
            if (feature.Error is not TException exception)
            {
                _logger.LogWarning("Invalid type of exception. Problem details cannot be serialized.");
                return;
            }

            Fill(context.ProblemDetails, exception, feature.Path);

            if (context.ProblemDetails.Status.HasValue) context.HttpContext.Response.StatusCode = context.ProblemDetails.Status.Value;
            await context.HttpContext.Response.WriteAsJsonAsync(context.ProblemDetails, options: null, MediaTypeNames.Application.Json.Problem, context.HttpContext.RequestAborted);
        }

        protected abstract void Fill(ProblemDetails problem, TException exception, string path);
    }
}

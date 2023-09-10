using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace STrain.CQS.NetCore.ExceptionHandling
{
    public class ProblemDetailsService : IProblemDetailsService
    {
        private readonly ILogger<ProblemDetailsService> _logger;

        public ProblemDetailsService(ILogger<ProblemDetailsService> logger)
        {
            _logger = logger;
        }

        public async ValueTask WriteAsync(ProblemDetailsContext context)
        {
            _logger.LogDebug("Selecting problem details writer");
            var writer = context.HttpContext.RequestServices.GetServices<IProblemDetailsWriter>().First(w => w.CanWrite(context));
            await writer.WriteAsync(context);
        }
    }
}

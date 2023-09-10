using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using STrain.CQS.NetCore.ErrorHandling;

namespace STrain.CQS.NetCore.ExceptionHandling
{
    public class ExceptionProblemDetailsWriter : BaseExceptionProblemDetailsWriter<Exception>
    {
        public ExceptionProblemDetailsWriter(ILogger<BaseExceptionProblemDetailsWriter<Exception>> logger) : base(logger)
        {
        }

        protected override void Fill(ProblemDetails problem, Exception exception, string path)
        {
            problem.Status = StatusCodes.Status500InternalServerError;
            problem.Type = ErrorEnumeration.InternalServerError.Type;
            problem.Title = ErrorEnumeration.InternalServerError.Title;
            problem.Detail = ErrorEnumeration.InternalServerError.Detail;
            problem.Instance = path;
        }
    }
}

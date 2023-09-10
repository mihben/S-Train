using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using STrain.Core.Exceptions;

namespace STrain.CQS.NetCore.ExceptionHandling
{
    public class VerificationExceptionProblemDetailsWriter : BaseExceptionProblemDetailsWriter<VerificationException>
    {
        public VerificationExceptionProblemDetailsWriter(ILogger<VerificationExceptionProblemDetailsWriter> logger) : base(logger)
        {
        }

        protected override void Fill(ProblemDetails problem, VerificationException exception, string path)
        {
            problem.Status = StatusCodes.Status400BadRequest;
            problem.Title = exception.Title;
            problem.Detail = exception.Detail;
            problem.Type = exception.Type;
            problem.Instance = path;
        }
    }
}

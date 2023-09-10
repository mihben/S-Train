using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using STrain.Core.Exceptions;

namespace STrain.CQS.NetCore.ExceptionHandling
{
    public class NotFoundExceptionProblemDetailsWriter : BaseExceptionProblemDetailsWriter<NotFoundException>
    {
        public NotFoundExceptionProblemDetailsWriter(ILogger<NotFoundExceptionProblemDetailsWriter> logger) : base(logger)
        {
        }

        protected override void Fill(ProblemDetails problem, NotFoundException exception, string path)
        {
            problem.Status = StatusCodes.Status404NotFound;
            problem.Type = exception.Type;
            problem.Title = exception.Title;
            problem.Detail = exception.Detail;
            problem.Instance = path;
        }
    }
}

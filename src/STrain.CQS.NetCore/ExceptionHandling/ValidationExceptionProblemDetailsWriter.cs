using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using STrain.Core.Exceptions;
using STrain.CQS.NetCore.ErrorHandling;

namespace STrain.CQS.NetCore.ExceptionHandling
{
    public class ValidationExceptionProblemDetailsWriter : BaseExceptionProblemDetailsWriter<ValidationException>
    {
        public ValidationExceptionProblemDetailsWriter(ILogger<ValidationExceptionProblemDetailsWriter> logger) : base(logger)
        {
        }

        protected override void Fill(ProblemDetails problem, ValidationException exception, string path)
        {
            problem.Status = StatusCodes.Status400BadRequest;
            problem.Type = ErrorEnumeration.Validation.Type;
            problem.Title = ErrorEnumeration.Validation.Title;
            problem.Detail = ErrorEnumeration.Validation.Detail;
            problem.Instance = path;
            problem.Extensions.Add("Errors", exception.Errors.Select(e => new { Property = e.Key, Message = e.Value}).ToList());
        }
    }
}

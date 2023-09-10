using AutoBogus;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using STrain.Core.Exceptions;
using STrain.CQS.NetCore.ExceptionHandling;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Unit.NetCore.ExceptionHandling
{
    public class VerificationExceptionProblemDetailsWriterTest
    {
        private readonly ILogger<VerificationExceptionProblemDetailsWriter> _logger;

        public VerificationExceptionProblemDetailsWriterTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                                .AddXUnit(outputHelper)
                                .CreateLogger<VerificationExceptionProblemDetailsWriter>();
        }

        private VerificationExceptionProblemDetailsWriter CreateSUT()
        {
            return new VerificationExceptionProblemDetailsWriter(_logger);
        }

        [Fact(DisplayName = "[UNIT][CEPDW-001] - Can write VerificationException")]
        public void VerificationExceptionProblemDetailsWriter_CanWrite_CanWriteVerificationException()
        {
            // Arrange
            var sut = CreateSUT();
            var exception = new AutoFaker<VerificationException>().Generate();

            // Act
            var result = sut.CanWrite(new ProblemDetailsContext { HttpContext = new HttpContextBuilder().WithException(exception).Build() });

            // Assert
            Assert.True(result);
        }

        [Fact(DisplayName = "[UNIT][CEPDW-002] - Cannot write Exception")]
        public void VerificationExceptionProblemDetailsWriter_CanWrite_CannotWriteException()
        {
            // Arrange
            var sut = CreateSUT();
            var exception = new AutoFaker<Exception>().Generate();

            // Act
            var result = sut.CanWrite(new ProblemDetailsContext { HttpContext = new HttpContextBuilder().WithException(exception).Build() });

            // Assert
            Assert.False(result);
        }

        [Fact(DisplayName = "[UNIT][CEPDW-003] - Write response")]
        public async Task VerificationExceptionProblemDetailsWriter_WriteAsync_WriteResponse()
        {
            // Arrange
            var sut = CreateSUT();
            var exception = new AutoFaker<VerificationException>().Generate();
            var context = new HttpContextBuilder().WithException(exception).Build();

            // Act
            await sut.WriteAsync(new ProblemDetailsContext { HttpContext = context });

            // Assert
            Assert.Equal(await context.Response.ReadFromJsonAsync<ProblemDetails>(), exception.AsProblemDetails(context.Features.GetRequiredFeature<IExceptionHandlerPathFeature>().Path), new ProblemDetailsEqualityComparer());
        }
    }

    file static class VerificationExceptionProblemDetailsWriterExtensions
    {
        public static ProblemDetails AsProblemDetails(this VerificationException exception, string path)
        {
            return new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = exception.Title,
                Detail = exception.Detail,
                Instance = path,
                Type = exception.Type
            };
        }
    }
}

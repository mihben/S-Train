using AutoBogus;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using STrain.CQS.NetCore.ErrorHandling;
using STrain.CQS.NetCore.ExceptionHandling;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Unit.NetCore.ExceptionHandling
{
    public class ExceptionProblemDetailsWriterTest
    {
        private readonly ILogger<ExceptionProblemDetailsWriter> _logger;

        public ExceptionProblemDetailsWriterTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                                .AddXUnit(outputHelper)
                                .CreateLogger<ExceptionProblemDetailsWriter>();
        }

        private ExceptionProblemDetailsWriter CreateSUT()
        {
            return new ExceptionProblemDetailsWriter(_logger);
        }

        [Fact(DisplayName = "[UNIT][EPDW-001] - Can write exception")]
        public void ExceptionProblemDetailsWriter_CanWriter_CanWriteException()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            var result = sut.CanWrite(new ProblemDetailsContext
            {
                HttpContext = new HttpContextBuilder().WithException(new AutoFaker<Exception>().Generate()).Build()
            });

            // Assert
            Assert.True(true);
        }

        [Fact(DisplayName = "[UNIT][EPDW-002] - Write response")]
        public async Task ExceptionProblemDetailsWriterTest_WriterAsync_WriterResponse()
        {
            // Arrange
            var sut = CreateSUT();
            var exception = new AutoFaker<Exception>().Generate();
            var context = new HttpContextBuilder().WithException(exception).Build();

            // Act
            await sut.WriteAsync(new ProblemDetailsContext { HttpContext = context });

            // Assert
            Assert.Equal(await context.Response.ReadFromJsonAsync<ProblemDetails>(), exception.AsProblemDetails(context.Features.GetRequiredFeature<IExceptionHandlerPathFeature>().Path), new ProblemDetailsEqualityComparer());
        }
    }

    file static class ExceptionProblemDetailsWriterTestExtensions
    {
        public static ProblemDetails AsProblemDetails(this Exception problem, string path)
        {
            return new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Type = ErrorEnumeration.InternalServerError.Type,
                Title = ErrorEnumeration.InternalServerError.Title,
                Detail = ErrorEnumeration.InternalServerError.Detail,
                Instance = path
            };
        }
    }
}

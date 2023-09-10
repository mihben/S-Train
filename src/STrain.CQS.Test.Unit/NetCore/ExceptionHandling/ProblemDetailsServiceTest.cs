using Microsoft.Extensions.Logging;
using STrain.CQS.NetCore.ExceptionHandling;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Http;
using Moq;
using Microsoft.AspNetCore.Mvc;
using STrain.CQS.NetCore.ErrorHandling;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Diagnostics;
using AutoBogus;

namespace STrain.CQS.Test.Unit.NetCore.ExceptionHandling
{
    public class ProblemDetailsServiceTest
    {
        private readonly ILogger<ProblemDetailsService> _logger;

        public ProblemDetailsServiceTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                            .AddXUnit(outputHelper)
                            .CreateLogger<ProblemDetailsService>();
        }

        private ProblemDetailsService CreateSUT()
        {
            return new ProblemDetailsService(_logger);
        }

        [Fact(DisplayName = "[UNIT][PDS-001] - Use problem details writer")]
        public async Task ProblemDetailsService_WriteAsync_UseProblemDetailsWriter()
        {
            // Arrange
            var sut = CreateSUT();
            var problemDetailsWriterMock = new Mock<IProblemDetailsWriter>();
            var context = new ProblemDetailsContext
            {
                HttpContext = new HttpContextBuilder()
                                    .Registrate(problemDetailsWriterMock.Object)
                                    .Build()
            };

            problemDetailsWriterMock.Setup(mock => mock.CanWrite(It.IsAny<ProblemDetailsContext>())).Returns(true);

            // Act
            await sut.WriteAsync(context);

            // Assert
            problemDetailsWriterMock.Verify(mock => mock.WriteAsync(context), Times.Once);
        }
    }

    file static class ProblemDetailsServiceTestExtensions
    {
        public static ProblemDetails AsUnexpectedError(this ProblemDetails problem, string path)
        {
            problem.Status = StatusCodes.Status500InternalServerError;
            problem.Type = ErrorEnumeration.InternalServerError.Type;
            problem.Title = ErrorEnumeration.InternalServerError.Title;
            problem.Detail= ErrorEnumeration.InternalServerError.Detail;
            problem.Instance = path;

            return problem;
        }
    }
}

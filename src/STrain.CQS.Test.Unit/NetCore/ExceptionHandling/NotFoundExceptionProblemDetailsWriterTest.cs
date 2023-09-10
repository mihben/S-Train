using AutoBogus;
using Bogus;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using STrain.Core.Exceptions;
using STrain.CQS.NetCore.ExceptionHandling;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Unit.NetCore.ExceptionHandling
{
    public class NotFoundExceptionProblemDetailsWriterTest
    {
        private readonly ILogger<NotFoundExceptionProblemDetailsWriter> _logger;

        public NotFoundExceptionProblemDetailsWriterTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                          .AddXUnit(outputHelper)
                          .CreateLogger<NotFoundExceptionProblemDetailsWriter>();
        }

        private NotFoundExceptionProblemDetailsWriter CreateSUT()
        {
            return new NotFoundExceptionProblemDetailsWriter(_logger);
        }

        [Fact(DisplayName = "[UNIT][NFPDW-001] - Can write NotFoundException")]
        public void NotFoundProblemDetailsWriter_CanWrite_NotFoundException()
        {
            // Arrange
            var sut = CreateSUT();
            var exception = new AutoFaker<NotFoundException>().Generate();

            // Act
            var result = sut.CanWrite(new ProblemDetailsContext { HttpContext = new HttpContextBuilder().WithException(exception).Build() });

            // Assert
            Assert.True(result);
        }

        [Fact(DisplayName = "[UNIT][NFPDW-002] - Cannot write Exception")]
        public void NotFoundProblemDetailsWriter_CannotWrite_Exception()
        {
            // Arrange
            var sut = CreateSUT();
            var exception = new AutoFaker<Exception>().Generate();

            // Act
            var result = sut.CanWrite(new ProblemDetailsContext { HttpContext = new HttpContextBuilder().WithException(exception).Build() });

            // Assert
            Assert.False(result);
        }

        [Fact(DisplayName = "[UNIT][NFPDW-003] - Write response")]
        public async Task NotFoundExceptionProblemDetailsWriter_WriteAsync_WriteResponse()
        {
            // Arrange
            var sut = CreateSUT();
            var exception = new AutoFaker<NotFoundException>().Generate();
            var context = new HttpContextBuilder().WithException(exception).Build();

            // Act
            await sut.WriteAsync(new ProblemDetailsContext { HttpContext = context });

            // Assert
            Assert.Equal(await context.Response.ReadFromJsonAsync<ProblemDetails>(), exception.AsProblemDetails(context.Features.GetRequiredFeature<IExceptionHandlerPathFeature>().Path), new ProblemDetailsEqualityComparer());
        }
    }

    file static class NotFoundProblemDetalisWriterTestExtensions
    {
        public static ProblemDetails AsProblemDetails(this NotFoundException exception, string path)
        {
            return new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = exception.Title,
                Detail = exception.Detail,
                Instance = path,
                Type = exception.Type
            };
        }
    }
}

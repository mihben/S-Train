using AutoBogus;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using STrain.Core.Exceptions;
using STrain.CQS.NetCore.ErrorHandling;
using STrain.CQS.NetCore.ExceptionHandling;
using System.Text.Json;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Unit.NetCore.ExceptionHandling
{
    public class ValidationExceptionProblemDetailsWriterTest
    {
        private readonly ILogger<ValidationExceptionProblemDetailsWriter> _logger;

        public ValidationExceptionProblemDetailsWriterTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                            .AddXUnit(outputHelper)
                            .CreateLogger<ValidationExceptionProblemDetailsWriter>();
        }

        private ValidationExceptionProblemDetailsWriter CreateSUT()
        {
            return new ValidationExceptionProblemDetailsWriter(_logger);
        }

        [Fact(DisplayName = "[UNIT][VEPDW-001] - Can write ValidationException")]
        public void ValidationExceptionProblemDetailsWriter_CanWrite_CanWriteValidationException()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            var result = sut.CanWrite(new ProblemDetailsContext
            {
                HttpContext = new HttpContextBuilder()
                    .WithException(new AutoFaker<ValidationException>().Generate())
                    .Build()
            });

            // Assert
            Assert.True(result);
        }

        [Fact(DisplayName = "[UNIT][VEPDW-002] - Cannot write Exception")]
        public void ValidationExceptionProblemDetailsWriter_CanWrite_CannotWriteException()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            var result = sut.CanWrite(new ProblemDetailsContext
            {
                HttpContext = new HttpContextBuilder()
                    .WithException(new AutoFaker<Exception>().Generate())
                    .Build()
            });

            // Assert
            Assert.False(result);
        }

        [Fact(DisplayName = "[UNIT][VEPDW-003] - Write response")]
        public async Task ValidationExceptionProblemDetailsWriter_WriteAsync_WriteResponse()
        {
            // Arrange
            var sut = CreateSUT();
            var exception = new AutoFaker<ValidationException>().Generate();
            var context = new HttpContextBuilder().WithException(exception).Build();

            // Act
            await sut.WriteAsync(new ProblemDetailsContext { HttpContext = context });

            // Assert
            var expected = exception.AsProblemDetails(context.Features.GetRequiredFeature<IExceptionHandlerPathFeature>().Path);
            var response = await context.Response.ReadFromJsonAsync<ProblemDetails>();
            Assert.Equal(expected, response, new ProblemDetailsEqualityComparer());
            Assert.Collection(response!.Extensions["Errors"].ReadFromJson(), expected.Extensions["Errors"].AsInspectors().ToArray());
        }
    }

    file static class ValidationExceptionProblemDetailsWriterTestExtensions
    {
        public static ProblemDetails AsProblemDetails(this ValidationException exception, string path)
        {
            var result = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Type = ErrorEnumeration.Validation.Type,
                Title = ErrorEnumeration.Validation.Title,
                Detail = ErrorEnumeration.Validation.Detail,
                Instance = path
            };
            result.Extensions.Add("Errors", exception.Errors.Select(e => new Error(e.Key, e.Value)).ToList());

            return result;
        }

        public static IEnumerable<Error> ReadFromJson(this object? errors)
        {
            return JsonSerializer.Deserialize<IEnumerable<Error>>((JsonElement)errors!, new JsonSerializerOptions { PropertyNameCaseInsensitive = true})!;
        }

        public static IEnumerable<Action<Error>> AsInspectors(this object? errors)
        {
            foreach (var error in (IEnumerable<Error>)errors!)
            {
                yield return e => Assert.Equal(error, e);
            }
        }
    }

    file record Error(string Property, string Message);
}

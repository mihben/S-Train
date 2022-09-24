using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using STrain.Core.Exceptions;
using STrain.CQS.Http.RequestSending;
using STrain.CQS.NetCore.ErrorHandling;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Unit.Http.RequestSending
{
    public class GenericRequestErrorHandlerTest
    {
        private readonly ILogger<GenericRequestErrorHandler> _logger;

        public GenericRequestErrorHandlerTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                            .AddXUnit(outputHelper)
                            .CreateLogger<GenericRequestErrorHandler>();
        }

        private GenericRequestErrorHandler CreateSUT()
        {
            return new GenericRequestErrorHandler(_logger);
        }

        [Fact(DisplayName = "[UNIT][GRH-001] - Invalid content type")]
        public async Task GenericResponseHandler_HandleErrorAsync_InvalidContentType()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            await Assert.ThrowsAsync<HttpRequestException>(async () => await sut.HandleAsync(new HttpResponseMessage(HttpStatusCode.NotFound), default));
        }

        [Fact(DisplayName = "[UNIT][GRH-002] - Not found")]
        public async Task GenericResponseHandler_HandleErrorAsync_NotFound()
        {
            // Arrange
            var sut = CreateSUT();
            var problem = new Fixture()
                            .Build<ProblemDetails>()
                                .With(pd => pd.Status, 404)
                            .Create();

            // Act
            // Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () => await sut.HandleAsync(new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = JsonContent.Create(problem, new MediaTypeHeaderValue(MediaTypeNames.Application.Json.Problem))
            }, default));
        }

        [Fact(DisplayName = "[UNIT][GRH-003] - Unathorized")]
        public async Task GenericResponseHandler_HandleErrorAsync_Unathorized()
        {
            // Arrange
            var sut = CreateSUT();
            var problem = new Fixture()
                            .Build<ProblemDetails>()
                                .With(pd => pd.Status, StatusCodes.Status401Unauthorized)
                            .Create();

            var response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = JsonContent.Create(problem, new MediaTypeHeaderValue(MediaTypeNames.Application.Json.Problem))
            };

            // Act
            // Assert
            await Assert.ThrowsAsync<HttpRequestException>(async () => await sut.HandleAsync(response, default));
        }

        [Fact(DisplayName = "[UNIT][GRH-004] - Forbidden")]
        public async Task GenericResponseHandler_HandleErrorAsync_Forbidden()
        {
            // Arrange
            var sut = CreateSUT();
            var problem = new Fixture()
                            .Build<ProblemDetails>()
                                .With(pd => pd.Status, StatusCodes.Status403Forbidden)
                            .Create();

            var response = new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = JsonContent.Create(problem, new MediaTypeHeaderValue(MediaTypeNames.Application.Json.Problem))
            };

            // Act
            // Assert
            await Assert.ThrowsAsync<HttpRequestException>(async () => await sut.HandleAsync(response, default));
        }

        [Fact(DisplayName = "[UNIT][GRH-005] - Verification error")]
        public async Task GenericResponseHandler_HandleErrorAsync_VerificationError()
        {
            // Arrange
            var sut = CreateSUT();
            var problem = new Fixture()
                            .Build<ProblemDetails>()
                                .With(pd => pd.Status, StatusCodes.Status400BadRequest)
                            .Create();

            var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = JsonContent.Create(problem, new MediaTypeHeaderValue(MediaTypeNames.Application.Json.Problem))
            };

            // Act
            // Assert
            await Assert.ThrowsAsync<VerificationException>(async () => await sut.HandleAsync(response, default));
        }

        [Fact(DisplayName = "[UNIT][GRH-006] - Validation error")]
        public async Task GenericResponseHandler_HandleErrorAsync_ValidationError()
        {
            // Arrange
            var sut = CreateSUT();
            var problem = new Fixture()
                            .Build<ProblemDetails>()
                                .With(pd => pd.Status, StatusCodes.Status400BadRequest)
                                .With(pd => pd.Type, ErrorEnumeration.Validation.Type)
                            .Create();

            var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = JsonContent.Create(problem, new MediaTypeHeaderValue(MediaTypeNames.Application.Json.Problem))
            };

            // Act
            // Assert
            await Assert.ThrowsAsync<ValidationException>(async () => await sut.HandleAsync(response, default));
        }
    }
}

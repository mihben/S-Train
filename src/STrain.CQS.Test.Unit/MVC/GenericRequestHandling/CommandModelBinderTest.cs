using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using STrain.CQS.MVC.GenericRequestHandling;
using STrain.CQS.Test.Unit.Supports;
using System.Text.Json;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Unit.MVC.GenericRequestHandling
{
    public class CommandModelBinderTest
    {
        private readonly ILogger<CommandModelBinder> _logger;

        public CommandModelBinderTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                            .AddXUnit(outputHelper)
                            .CreateLogger<CommandModelBinder>();
        }

        private CommandModelBinder CreateSUT()
        {
            return new CommandModelBinder(_logger);
        }

        [Fact(DisplayName = "[UNIT][CMB-001]: Bind based on 'Request-Type' header")]
        public async Task CommandModelBinder_BindModelAsync_BindBasedOnRequestTypeHeader()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new TestCommand(new Fixture().Create<string>());
            var modelBindingContextMock = new Mock<ModelBindingContext>();

            modelBindingContextMock.MockHttpContext()
                .UseHeaders(new Dictionary<string, StringValues> { ["Request-Type"] = "STrain.CQS.Test.Unit.Supports.TestCommand, STrain.CQS.Test.Unit" })
                .UseBody(command);

            // Act
            await sut.BindModelAsync(modelBindingContextMock.Object);

            // Assert
            modelBindingContextMock.VerifySet(mbc => mbc.Result = ModelBindingResult.Success(command));
        }

        [Fact(DisplayName = "[UNIT][CMB-002]: Bind based on target type")]
        public async Task CommandModelBinder_BindModelAsync_BindBasedOnTargetType()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new TestCommand(new Fixture().Create<string>());
            var modelBindingContextMock = new Mock<ModelBindingContext>();

            modelBindingContextMock.SetModelType<TestCommand>();
            modelBindingContextMock.MockHttpContext()
                .UseHeaders(new Dictionary<string, StringValues>())
                .UseBody(command);

            // Act
            await sut.BindModelAsync(modelBindingContextMock.Object);

            // Assert
            modelBindingContextMock.VerifySet(mbc => mbc.Result = ModelBindingResult.Success(command));
        }

        [Fact(DisplayName = "[UNIT][CMB-003]: Unknown request type")]
        public async Task CommandModelBinder_BindModelAsync_UnknownRequestType()
        {
            // Arrange
            var sut = CreateSUT();
            var modelBindingContextMock = new Mock<ModelBindingContext>();

            modelBindingContextMock.MockHttpContext()
                .UseHeaders(new Dictionary<string, StringValues> { ["Request-Type"] = "FakeType" });

            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.BindModelAsync(modelBindingContextMock.Object));
        }
    }

    internal static class CommandModelBinderTestExtensions
    {
        public static Mock<ModelBindingContext> SetModelType<T>(this Mock<ModelBindingContext> mock) => mock.SetModelType(typeof(T));
        public static Mock<ModelBindingContext> SetModelType(this Mock<ModelBindingContext> mock, Type type)
        {
            mock.SetupGet(mbc => mbc.ModelType)
                .Returns(type);

            return mock;
        }

        public static ModelBindingContextBuilder MockHttpContext(this Mock<ModelBindingContext> context)
        {
            var httpContextMock = new Mock<HttpContext>();
            var httpRequestMock = new Mock<HttpRequest>();

            httpContextMock.SetupGet(hc => hc.Request)
                .Returns(httpRequestMock.Object);
            context.SetupGet(c => c.HttpContext)
                .Returns(httpContextMock.Object);

            return new ModelBindingContextBuilder(httpRequestMock);
        }
    }

    internal class ModelBindingContextBuilder
    {
        private readonly Mock<HttpRequest> _httpRequestMock;

        public ModelBindingContextBuilder(Mock<HttpRequest> httpRequestMock)
        {
            _httpRequestMock = httpRequestMock;
        }

        public ModelBindingContextBuilder UseHeaders(Dictionary<string, StringValues> headers)
        {
            _httpRequestMock.SetupGet(hr => hr.Headers)
                .Returns(new HeaderDictionary(headers));

            return this;
        }

        public ModelBindingContextBuilder UseBody<TRequest>(TRequest request)
        {
            var stream = new MemoryStream();
            JsonSerializer.Serialize(stream, request);
            stream.Position = 0;

            _httpRequestMock.SetupGet(hr => hr.Body)
                .Returns(stream);

            return this;
        }
    }
}

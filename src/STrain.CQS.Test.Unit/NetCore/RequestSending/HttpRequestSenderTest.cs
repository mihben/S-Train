using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using STrain.CQS.NetCore.RequestSending;
using STrain.CQS.Test.Unit.Supports;
using System.Net;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Unit.NetCore.RequestSending
{
    public class HttpRequestSenderTest
    {
        private readonly ILogger<HttpRequestSender> _logger;

        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private Mock<IPathProvider> _pathProviderMock;
        private Mock<IMethodProvider> _methodProviderMock;

        public HttpRequestSenderTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                            .AddXUnit(outputHelper)
                            .CreateLogger<HttpRequestSender>();
        }

        private HttpRequestSender CreateSUT()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _pathProviderMock = new Mock<IPathProvider>();
            _methodProviderMock = new Mock<IMethodProvider>();

            _httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK });
            var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("http://test-elek.com/")
            };

            return new HttpRequestSender(httpClient, _pathProviderMock.Object, _methodProviderMock.Object, _logger);
        }

        [Fact(DisplayName = "[UNIT][HRS-001] - Send command")]
        public async Task HttpRequestSender_SendAsync_SendCommand()
        {
            // Arrange
            var sut = CreateSUT();
            var path = new Fixture().Create<string>();
            var method = HttpMethod.Post;

            _pathProviderMock.Setup(pp => pp.GetPath(It.IsAny<TestCommand>()))
                .Returns(path);
            _methodProviderMock.Setup(mp => mp.GetMethod<TestCommand>())
                .Returns(HttpMethod.Delete);

            // Act
            await sut.SendAsync(new Fixture().Create<TestCommand>(), default);

            // Assert
            _httpMessageHandlerMock.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(hrm => hrm.Verify(path, HttpMethod.Delete)), ItExpr.IsAny<CancellationToken>());
        }
    }

    internal static class HttpRequestSenderExtensions
    {
        public static bool Verify(this HttpRequestMessage message, string path, HttpMethod method)
        {
            return message.RequestUri.AbsolutePath.Equals($"/{path}") &&
                message.Method.Equals(method);
        }
    }
}

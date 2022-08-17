using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using STrain.CQS.Senders;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Unit.CQS
{
    public class RequestRouterTest
    {
        private readonly ILogger<RequestRouter> _logger;
        private Mock<IServiceProvider> _serviceProviderMock;

        public RequestRouterTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<RequestRouter>();
        }

        private RequestRouter CreateSUT(Func<IRequest, string> senderKeyProvider)
        {
            _serviceProviderMock = new Mock<IServiceProvider>();

            return new RequestRouter(_serviceProviderMock.Object, senderKeyProvider, _logger);
        }

        [Fact(DisplayName = "[UNIT][RQR-001] - Send request")]
        public async Task RequestRouter_SendAsync_SendRequest()
        {
            // Arrange
            var key = new Fixture().Create<string>();
            var sut = CreateSUT((_) => key);
            var requestSenderMock = new Mock<IRequestSender>();
            var request = new TestRequest();

            _serviceProviderMock.Setup(sp => sp.GetService(typeof(Func<string, IRequestSender>()))
                .Returns(_ => requestSenderMock.Object);

            // Act
            await sut.SendAsync<TestRequest, object>(request, default);

            // Assert
            requestSenderMock.Verify(rs => rs.SendAsync<TestRequest, object>(request, It.IsAny<CancellationToken>()));
        }
    }

    internal record TestRequest : IRequest { }
}

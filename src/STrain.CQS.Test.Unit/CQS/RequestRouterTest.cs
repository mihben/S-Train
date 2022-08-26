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

        public RequestRouterTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<RequestRouter>();
        }

        private RequestRouter CreateSUT(Func<string, IRequestSender> senderFactory, Func<IRequest, string> senderKeyProvider)
        {
            return new RequestRouter(senderFactory, senderKeyProvider, _logger);
        }

        [Fact(DisplayName = "[UNIT][RQR-001] - Send request")]
        public async Task RequestRouter_SendAsync_SendRequest()
        {
            // Arrange
            var key = new Fixture().Create<string>();
            var requestSenderMock = new Mock<IRequestSender>();
            var sut = CreateSUT(_ => requestSenderMock.Object, (_) => key);
            var request = new TestRequest();

            // Act
            await sut.SendAsync<TestRequest, object>(request, default);

            // Assert
            requestSenderMock.Verify(rs => rs.SendAsync<TestRequest, object>(request, It.IsAny<CancellationToken>()));
        }

        [Fact(DisplayName = "[UNIT][RQR-002] - Request is null")]
        public async Task RequestRouter_SendAsync_RequestIsNull()
        {
            // Arrange
#pragma warning disable CS8603 // Possible null reference return.
            var sut = CreateSUT(_ => null, (_) => null);
#pragma warning restore CS8603 // Possible null reference return.

            // Act
            // Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.SendAsync<TestRequest, object>(null, default));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact(DisplayName = "[UNIT][RQR-003] - Key not found")]
        public async Task RequestRouter_SendAsync_KeyNotFound()
        {
            // Arrange
#pragma warning disable CS8603 // Possible null reference return.
            var sut = CreateSUT(_ => null, (_) => null);
#pragma warning restore CS8603 // Possible null reference return.

            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.SendAsync<TestRequest, object>(new TestRequest(), default));
        }

        [Fact(DisplayName = "[UNIT][RQR-004] - Request sender is not registered with key")]
        public async Task RequestRouter_SendAsync_RequestSenderIsNotRegisteredWithKey()
        {
            // Arrange
            var key = new Fixture().Create<string>();
#pragma warning disable CS8603 // Possible null reference return.
            var sut = CreateSUT(_ => null, (_) => key);
#pragma warning restore CS8603 // Possible null reference return.

            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.SendAsync<TestRequest, object>(new TestRequest(), default));
        }
    }

    internal record TestRequest : IRequest { }
}

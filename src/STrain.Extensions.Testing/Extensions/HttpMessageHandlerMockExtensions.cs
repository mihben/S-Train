using Moq.Protected;
using System.Linq.Expressions;

namespace Moq
{
    public static class HttpMessageHandlerMockExtensions
    {
        public static void VerifySend(this Mock<HttpMessageHandler> mock, Expression<Func<HttpRequestMessage, bool>> match) => mock.VerifySend(match, Times.Once());
        public static void VerifySend(this Mock<HttpMessageHandler> mock, Expression<Func<HttpRequestMessage, bool>> match, Times times)
        {
            mock.Protected().Verify("SendAsync", times, ItExpr.Is(match), ItExpr.IsAny<CancellationToken>());
        }
    }
}

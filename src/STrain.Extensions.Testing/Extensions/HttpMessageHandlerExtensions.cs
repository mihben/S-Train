using Moq.Protected;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Json;

namespace Moq
{
    public static class HttpMessageHandlerExtensions
    {
        public static Mock<HttpMessageHandler> SetupSend(this Mock<HttpMessageHandler> mock) => mock.SetupSend(_ => true, new HttpResponseMessage { StatusCode = HttpStatusCode.OK });
        public static Mock<HttpMessageHandler> SetupSend(this Mock<HttpMessageHandler> mock, Expression<Func<HttpRequestMessage, bool>> match) => mock.SetupSend(match, new HttpResponseMessage { StatusCode = HttpStatusCode.OK });
        public static Mock<HttpMessageHandler> SetupSend<T>(this Mock<HttpMessageHandler> mock, HttpStatusCode statusCode, T content) => mock.SetupSend(_ => true, statusCode, content);
        public static Mock<HttpMessageHandler> SetupSend<T>(this Mock<HttpMessageHandler> mock, Expression<Func<HttpRequestMessage, bool>> match, HttpStatusCode statusCode, T content) => mock.SetupSend(match, new HttpResponseMessage { StatusCode = statusCode, Content = JsonContent.Create(content) });
        public static Mock<HttpMessageHandler> SetupSend(this Mock<HttpMessageHandler> mock, Expression<Func<HttpRequestMessage, bool>> match, HttpResponseMessage response)
        {
            mock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is(match), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(response));
            return mock;
        }
    }
}

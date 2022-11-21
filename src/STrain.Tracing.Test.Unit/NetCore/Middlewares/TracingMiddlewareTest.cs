using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using STrain.Tracing.Core.Contexts;
using STrain.Tracing.NetCore.Middlewares;
using Xunit.Abstractions;

namespace STrain.Tracing.Test.Unit.NetCore.Middlewares
{
    public class TracingMiddlewareTest
    {

        private readonly ILogger<TracingMiddleware> _logger;
        private TracingContext _context = null!;
        private Mock<RequestDelegate> _nextMock = null!;

        public TracingMiddlewareTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                          .AddXUnit(outputHelper)
                          .CreateLogger<TracingMiddleware>();
        }

        private TracingMiddleware CreateSUT()
        {
            _context = new TracingContext();
            _nextMock = new Mock<RequestDelegate>();

            return new TracingMiddleware(_context, _nextMock.Object, _logger);
        }

        [Fact(DisplayName = "[UNIT][TMW-001] - Read 'Request-Id' header")]
        public async Task TracingMiddleware_InvokeAsync_ReadRequestIdHeader()
        {
            // Arrange
            var sut = CreateSUT();
            var context = new DefaultHttpContext();
            var requestId = Guid.NewGuid();

            context.Request.Headers.Add("Request-Id", requestId.ToString());

            // Act
            await sut.InvokeAsync(context);

            // Assert
            Assert.Equal(requestId, _context.RequestId);
        }

        [Fact(DisplayName = "[UNIT][TMW-002] - Generate 'RequestId'")]
        public async Task TracingMiddleware_InvokeAsync_GenerateRequestId()
        {
            // Arrange
            var sut = CreateSUT();
            var context = new DefaultHttpContext();

            // Act
            await sut.InvokeAsync(context);

            // Assert
            Assert.NotNull(_context.RequestId);
        }

        [Fact(DisplayName = "[UNIT][TMW-002] - Read 'Correlation-Id' header")]
        public async Task TracingMiddleware_InvokeAsync_ReadCorrelationIdHeader()
        {
            // Arrange
            var sut = CreateSUT();
            var context = new DefaultHttpContext();
            var correlationId = Guid.NewGuid();

            context.Request.Headers.Add("Correlation-Id", correlationId.ToString());

            // Act
            await sut.InvokeAsync(context);

            // Assert
            Assert.Equal(correlationId, _context.CorrelationId);
        }

        [Fact(DisplayName = "[UNIT][TMW-003] - Generate 'Correlation-Id'")]
        public async Task TracingMiddleware_InvokeAsync_GenerateCorrelationId()
        {
            // Arrange
            var sut = CreateSUT();
            var context = new DefaultHttpContext();

            // Act
            await sut.InvokeAsync(context);

            // Assert
            Assert.NotNull(_context.CorrelationId);
        }

        [Fact(DisplayName = "[UNIT][TMW-004] - Read 'Causation-Id' header")]
        public async Task TracingMiddleware_InvokeAsync_ReadCausationId()
        {
            // Arrange
            var sut = CreateSUT();
            var context = new DefaultHttpContext();
            var causationId = Guid.NewGuid();

            context.Request.Headers.Add("Causation-Id", causationId.ToString());

            // Act
            await sut.InvokeAsync(context);

            // Assert
            Assert.Equal(causationId, _context.CausationId);
        }

        [Fact(DisplayName = "[UNIT][TMW-005] - Null 'Causation-Id' header")]
        public async Task TracingMiddleware_InvokeAsync_NullCausationIdHeader()
        {
            // Arrange
            var sut = CreateSUT();
            var context = new DefaultHttpContext();

            // Act
            await sut.InvokeAsync(context);

            // Assert
            Assert.Null(_context.CausationId);
        }
    }
}

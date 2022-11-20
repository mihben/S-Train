using Microsoft.Extensions.Logging;
using STrain.CQS;
using STrain.Tracing.Core.Contexts;
using STrain.Tracing.Http.Binders;
using Xunit.Abstractions;
namespace STrain.Tracing.Test.Unit.Http.Binders
{
    public class TracingHeaderParameterBinderTest
    {

        private readonly ILogger<TracingHeaderParameterBinder> _logger;
        private TracingContext _context;

        public TracingHeaderParameterBinderTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                          .AddXUnit(outputHelper)
                          .CreateLogger<TracingHeaderParameterBinder>();
        }

        private TracingHeaderParameterBinder CreateSUT()
        {
            _context = new TracingContext();

            return new TracingHeaderParameterBinder(_context, _logger);
        }

        [Fact(DisplayName = "[UNIT][THPB-001] - Set 'Request-Id' header")]
        public async Task TracingHeaderParameterBinder_BindAsync_SetRequestIdHeader()
        {
            // Arrange
            var sut = CreateSUT();
            var message = new HttpRequestMessage();

            // Act
            await sut.BindAsync(new TestRequest(), message.Headers, default);

            // Assert
            Assert.Contains(message.Headers, h => h.Key.Equals("Request-Id") && h.Value.All(v => !Guid.Parse(v).Equals(Guid.Empty)));
        }

        [Fact(DisplayName = "[UNIT][THPB-002] - Set 'Correlation-Id' header")]
        public async Task TracingHeaderParameterBinder_BindAsync_SetCorrelationIdHeader()
        {
            // Arrange
            var sut = CreateSUT();
            var message = new HttpRequestMessage();

            _context.CorrelationId = Guid.NewGuid();

            // Act
            await sut.BindAsync(new TestRequest(), message.Headers, default);

            // Assert
            Assert.Contains(message.Headers, h => h.Key.Equals("Correlation-Id") && h.Value.Any(v => Guid.Parse(v).Equals(_context.CorrelationId)));
        }

        [Fact(DisplayName = "[UNIT][THPB-003] - Set 'Causation-Id' header")]
        public async Task TracingHeaderParameterBinder_BindAsync_SetCausationIdHeader()
        {
            // Arrange
            var sut = CreateSUT();
            var message = new HttpRequestMessage();

            _context.RequestId = Guid.NewGuid();

            // Act
            await sut.BindAsync(new TestRequest(), message.Headers, default);

            // Assert
            Assert.Contains(message.Headers, h => h.Key.Equals("Causation-Id") && h.Value.Any(v => Guid.Parse(v).Equals(_context.RequestId)));
        }

        [Fact(DisplayName = "[UNIT][THPB-003] - Generate Correlation-Id")]
        public async Task TracingHeaderParameterBinder_BindAsync_GenerateCorrelationId()
        {
            // Arrange
            var sut = CreateSUT();
            var message = new HttpRequestMessage();

            // Act
            await sut.BindAsync(new TestRequest(), message.Headers, default);

            // Assert
            Assert.Contains(message.Headers, h => h.Key.Equals("Correlation-Id") && h.Value.All(v => v is not null));
        }

        #region Models
        public record TestRequest : IRequest { }
        #endregion
    }
}

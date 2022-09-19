using AutoFixture;
using Microsoft.Extensions.Logging;
using STrain.CQS.Http.RequestSending.Binders.Attributive;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Unit.Http.RequestSending
{
    public class AttributiveRouteBinderTest
    {
        private readonly ILogger<AttributiveRouteBinder> _logger;

        public AttributiveRouteBinderTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                            .AddXUnit(outputHelper)
                            .CreateLogger<AttributiveRouteBinder>();
        }

        private AttributiveRouteBinder CreateSUT()
        {
            return new AttributiveRouteBinder(_logger);
        }

        [Fact(DisplayName = "[UNIT][ARB-001] - Bind to path")]
        public async Task AttributiveRouteBinder_BindAsync_BindToPath()
        {
            // Arrange
            var sut = CreateSUT();
            var request = new Fixture().Create<RoutedRequest>();

            // Act
            var result = await sut.BindAsync(request, default);

            // Assert
            Assert.Equal($"test-route/{request.Id}", result);
        }

        [Fact(DisplayName = "[UNIT][ARB-002] - Route attribute not found")]
        public async Task AttributiveRouteBinder_BindAsync_RouteAttributeNotFound()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.BindAsync(new Fixture().Create<UnroutedRequest>(), default));
        }

        [Fact(DisplayName = "[UNIT][ARB-003] - Route property not found")]
        public async Task AttributiveRouteBinder_BindAsync_RoutePropertyNotFound()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.BindAsync(new Fixture().Create<InvalidRoutedRequest>(), default));
        }

        [Fact(DisplayName = "[UNIT][ARB-004] - Null routed value")]
        public async Task AttributiveRouteBinder_BindAsync_NullRoutedValue()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.BindAsync(new Fixture().Build<RoutedRequest>().Without(r => r.Id).Create(), default));
        }

        [Fact(DisplayName = "[UNIT][ARB-005] - Request is null")]
        public async Task AttributiveRouteBinder_BindAsync_RequestIsNull()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.BindAsync<IRequest>(null!, default));
        }

        private record UnroutedRequest : IRequest { }

        [Route("test-route/{id}")]
        private record RoutedRequest : IRequest
        {
            public string Id { get; set; } = null!;
        }

        [Route("test-route/{name}")]
        private record InvalidRoutedRequest : IRequest
        {
            public string Id { get; set; } = null!;
        }
    }
}

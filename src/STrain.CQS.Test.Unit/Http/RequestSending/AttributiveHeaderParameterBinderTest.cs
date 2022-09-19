using AutoFixture;
using Microsoft.Extensions.Logging;
using STrain.CQS.Attributes.RequestSending.Http.Parameters;
using STrain.CQS.Http.RequestSending.Binders.Attributive;
using STrain.CQS.Test.Unit.CQS;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Unit.Http.RequestSending
{
    public class AttributiveHeaderParameterBinderTest
    {

        private readonly ILogger<AttributiveHeaderParameterBinder> _logger;

        public AttributiveHeaderParameterBinderTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                          .AddXUnit(outputHelper)
                          .CreateLogger<AttributiveHeaderParameterBinder>();
        }

        private AttributiveHeaderParameterBinder CreateSUT()
        {
            return new AttributiveHeaderParameterBinder(_logger);
        }

        [Fact(DisplayName = "[UNIT][AHPB-001] - Bind request")]
        public async Task AttributiveHeaderParameterBinder_BindAsync_BindObject()
        {
            // Arrange
            var sut = CreateSUT();
            var message = new HttpRequestMessage();
            var request = new Fixture().Create<HeaderParameterRequest>();

            // Act
            await sut.BindAsync(request, message.Headers, default);

            // Assert
            Assert.Collection(message.Headers, h => Assert.True(h.Key.Equals(nameof(HeaderParameterRequest.ByName)) && h.Value.First().Equals(request.ByName)),
                                                                       h => Assert.True(h.Key.Equals("by-attribute") && h.Value.First().Equals(request.ByAttribute)));
        }

        [Fact(DisplayName = "[UNIT][AHPB-002] - Bind propertes")]
        public async Task AttributiveHeaderParameterBinder_BindAsync_BindProperties()
        {
            // Arrange
            var sut = CreateSUT();
            var message = new HttpRequestMessage();
            var request = new Fixture().Create<HeaderPropertyParameterRequest>();

            // Act
            await sut.BindAsync(request, message.Headers, default);

            // Assert
            Assert.Collection(message.Headers, h => Assert.True(h.Key.Equals(nameof(HeaderPropertyParameterRequest.ByName)) && h.Value.First().Equals(request.ByName)),
                                                                       h => Assert.True(h.Key.Equals("by-attribute") && h.Value.First().Equals(request.ByAttribute)));
        }

        [Fact(DisplayName = "[UNIT][AHPB-003] - Skip null property")]
        public async Task AttributiveHeaderParameterBinder_BindAsync_SkipNullProperties()
        {
            // Arrange
            var sut = CreateSUT();
            var message = new HttpRequestMessage();
            var request = new Fixture().Build<HeaderParameterRequest>().Without(r => r.ByName).Without(r => r.ByAttribute).Create();

            // Act
            await sut.BindAsync(request, message.Headers, default);

            // Assert
            Assert.Empty(message.Headers);
        }

        [Fact(DisplayName = "[UNIT][AHPB-004] - Without header parameter")]
        public async Task AttributiveHeaderParameterBinder_BindAsync_WithoutHeaderParameter()
        {
            // Arrange
            var sut = CreateSUT();
            var message = new HttpRequestMessage();
            var request = new Fixture().Create<TestRequest>();

            // Act
            await sut.BindAsync(request, message.Headers, default);

            // Assert
            Assert.Empty(message.Headers);
        }

        [Fact(DisplayName = "[UNIT][AHPB-005] - Request is null")]
        public async Task AttributiveHeaderParameterBinder_BindAsync_RequestIsNull()
        {
            // Arrange
            var sut = CreateSUT();
            var message = new HttpRequestMessage();

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.BindAsync<IRequest>(null!, message.Headers, default));
        }

        [Fact(DisplayName = "[UNIT][AHPB-006] - Header is null")]
        public async Task AttributiveHeaderParameterBinder_BindAsync_HeaderIsNull()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.BindAsync(new Fixture().Create<TestRequest>(), null!, default));
        }

        [HeaderParameter]
        private record HeaderParameterRequest : IRequest
        {
            public string ByName { get; set; } = null!;
            [HeaderParameter(Name = "by-attribute")]
            public string ByAttribute { get; set; } = null!;
        }

        private record HeaderPropertyParameterRequest : IRequest
        {
            [HeaderParameter]
            public string ByName { get; set; } = null!;
            [HeaderParameter(Name = "by-attribute")]
            public string ByAttribute { get; set; } = null!;
            public string NotToBeSerialized { get; set; } = null!;
        }
    }
}

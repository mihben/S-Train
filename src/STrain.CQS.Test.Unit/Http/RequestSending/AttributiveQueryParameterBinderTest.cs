using AutoFixture;
using Microsoft.Extensions.Logging;
using STrain.CQS.Http.RequestSending.Binders.Attributive;
using STrain.CQS.Test.Unit.CQS;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Unit.Http.RequestSending
{
    public class AttributiveQueryParameterBinderTest
    {
        private readonly ILogger<AttributiveQueryParameterBinder> _logger;

        public AttributiveQueryParameterBinderTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                            .AddXUnit(outputHelper)
                            .CreateLogger<AttributiveQueryParameterBinder>();
        }

        private AttributiveQueryParameterBinder CreateSUT()
        {
            return new AttributiveQueryParameterBinder(_logger);
        }

        [Fact(DisplayName = "[UNIT][AQPB-001] - Bind whole object")]
        public async Task AttributiveQueryParameterBinder_BindAsync_BindQueryParameter()
        {
            // Arrange
            var sut = CreateSUT();
            var request = new Fixture().Create<QueryParameterRequest>();

            // Act
            var result = await sut.BindAsync(request, default);

            // Assert
            Assert.Equal($"{nameof(QueryParameterRequest.ByName)}={request.ByName}&by-attribute={request.ByAttribute}", result);
        }

        [Fact(DisplayName = "[UNIT][AQPB-002] - Bind property")]
        public async Task AttributiveQueryParameterBinder_BindAsync_BindProperty()
        {
            // Arrange
            var sut = CreateSUT();
            var request = new Fixture().Create<PropertyQueryParameterRequest>();

            // Act
            var result = await sut.BindAsync(request, default);

            // Assert
            Assert.Equal($"{nameof(PropertyQueryParameterRequest.ByName)}={request.ByName}&by-attribute={request.ByAttribute}", result);
        }

        [Fact(DisplayName = "[UNIT][AQPB-003] - Skip null value")]
        public async Task AttributiveQueryParameterBinder_BindAsync_SkipNullValue()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            var result = await sut.BindAsync(new Fixture().Build<QueryParameterRequest>().Without(r => r.ByName).Without(r => r.ByAttribute).Create(), default);

            // Assert
            Assert.Empty(result);
        }

        [Fact(DisplayName = "[UNIT][AQPB-004] - Without query parameter")]
        public async Task AttributiveQueryParameterBinder_BindAsync_WithoutQueryParameter()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            var result = await sut.BindAsync(new Fixture().Create<TestRequest>(), default);

            // Assert
            Assert.Empty(result);
        }

        [QueryParameter]
        private record QueryParameterRequest : IRequest
        {
            public string ByName { get; set; }
            [QueryParameter(Name = "by-attribute")]
            public string ByAttribute { get; set; }
        }

        private record PropertyQueryParameterRequest : IRequest
        {
            [QueryParameter]
            public string ByName { get; set; }
            [QueryParameter(Name = "by-attribute")]
            public string ByAttribute { get; set; }
            public string NotToBeSerialized { get; set; }
        }
    }
}

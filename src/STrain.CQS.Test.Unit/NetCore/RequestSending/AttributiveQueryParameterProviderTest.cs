using AutoFixture;
using Microsoft.Extensions.Logging;
using STrain.CQS.NetCore.RequestSending.Attributive;
using STrain.CQS.Test.Unit.Supports;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Unit.NetCore.RequestSending
{
    public class AttributiveQueryParameterProviderTest
    {
        private readonly ILogger<AttributiveQueryParameterProvider> _logger;

        public AttributiveQueryParameterProviderTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                            .AddXUnit(outputHelper)
                            .CreateLogger<AttributiveQueryParameterProvider>();
        }

        private AttributiveQueryParameterProvider CreateSUT()
        {
            return new AttributiveQueryParameterProvider(_logger);
        }

        [Fact(DisplayName = "[UNIT][ABQPP-001] - Add whole object to query parameter")]
        public async Task AttributeBasedQueryParameterProvider_SetParameterAsync_AddWholeObjectToQueryParameter()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new Fixture().Create<QueryParameterRequest>();
            var message = new HttpRequestMessage(HttpMethod.Get, "http://test.com");

            // Act
            await sut.SetParametersAsync(message, command, default);

            // Assert
            Assert.Equal($"?parameter={command.Parameter}", message.RequestUri?.Query);
        }

        [Fact(DisplayName = "[UNIT][ABQPP-002] - Add Property for Query Parameter")]
        public async Task AttributeBasedQueryParameterProvider_SetParameterAsync_AddPropertyForQueryParameter()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new Fixture().Create<PropertyQueryParameterRequest>();
            var message = new HttpRequestMessage(HttpMethod.Get, "http://test.com");

            // Act
            await sut.SetParametersAsync(message, command, default);

            // Assert
            Assert.Equal($"?parameter={command.Parameter}", message.RequestUri?.Query);
        }

        [Fact(DisplayName = "[UNIT][ABQPP-003] - Add Property for Query Parameter with Name")]
        public async Task AttributeBasedQueryParameterProvider_SetParameterAsync_AddPropertyForQueryParameterWithName()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new Fixture().Create<PropertyNameQueryParameterRequest>();
            var message = new HttpRequestMessage(HttpMethod.Get, "http://test.com");

            // Act
            await sut.SetParametersAsync(message, command, default);

            // Assert
            Assert.Equal($"?overridedparameter={command.Parameter}", message.RequestUri?.Query);
        }

        [Fact(DisplayName = "[UNIT][ABQPP-004] - Uri is null")]
        public async Task AttributeBasedQueryParameterProvider_SetParameterAsync_UriIsNull()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new Fixture().Create<PropertyNameQueryParameterRequest>();
            var message = new HttpRequestMessage();

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await sut.SetParametersAsync(message, command, default));
        }

        [Fact(DisplayName = "[UNIT][ABQPP-004] - Generic request")]
        public async Task AttributeBasedQueryParameterProvider_SetParameterAsync_GenericRequest()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new Fixture().Create<TestCommand>();
            var message = new HttpRequestMessage(HttpMethod.Get, "http://test.com");

            // Act
            await sut.SetParametersAsync(message, command, default);

            // Assert
            Assert.Empty(message.RequestUri?.Query);
        }
    }

    [Get]
    [QueryParameter]
    public record QueryParameterRequest : Command
    {
        public string Parameter { get; }

        public QueryParameterRequest(string parameter)
        {
            Parameter = parameter;
        }
    }

    [Get]
    public record PropertyQueryParameterRequest : Command
    {
        [QueryParameter]
        public string Parameter { get; }

        public PropertyQueryParameterRequest(string parameter)
        {
            Parameter = parameter;
        }
    }

    [Get]
    public record PropertyNameQueryParameterRequest : Command
    {
        [QueryParameter(Name = "OverridedParameter")]
        public string Parameter { get; }

        public PropertyNameQueryParameterRequest(string parameter)
        {
            Parameter = parameter;
        }
    }
}

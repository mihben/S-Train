using AutoFixture;
using Moq;
using STrain.CQS.Attributes.RequestSending.Http;
using STrain.CQS.NetCore.RequestSending;

namespace STrain.CQS.Test.Unit.NetCore.RequestSending
{
    public class AttributeBasedQueryParameterProviderTest
    {
        private AttributeBasedQueryParameterProvider CreateSUT()
        {
            return new AttributeBasedQueryParameterProvider();
        }

        [Fact(DisplayName = "[UNIT][ABQPP-001] - Add Whole Object to Query Parameter")]
        public async Task AttributeBasedQueryParameterProvider_SetParameterAsync_AddWholeObjectToQueryParameter()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new Fixture().Create<WholeObjectTestCommand>();
            var message = new HttpRequestMessage(HttpMethod.Get, "http://test.com");

            // Act
            await sut.SetParametersAsync(message, command, default);

            // Assert
            Assert.Equal($"?parameter={command.Parameter}", message.RequestUri.Query);
        }

        [Fact(DisplayName = "[UNIT][ABQPP-002] - Add Property for Query Parameter")]
        public async Task AttributeBasedQueryParameterProvider_SetParameterAsync_AddPropertyForQueryParameter()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new Fixture().Create<PropertyTestCommand>();
            var message = new HttpRequestMessage(HttpMethod.Get, "http://test.com");

            // Act
            await sut.SetParametersAsync(message, command, default);

            // Assert
            Assert.Equal($"?parameter={command.Parameter}", message.RequestUri.Query);
        }

        [Fact(DisplayName = "[UNIT][ABQPP-003] - Add Property for Query Parameter with Name")]
        public async Task AttributeBasedQueryParameterProvider_SetParameterAsync_AddPropertyForQueryParameterWithName()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new Fixture().Create<PropertyNameTestCommand>();
            var message = new HttpRequestMessage(HttpMethod.Get, "http://test.com");

            // Act
            await sut.SetParametersAsync(message, command, default);

            // Assert
            Assert.Equal($"?overridedparameter={command.Parameter}", message.RequestUri.Query);
        }

        [Fact(DisplayName = "[UNIT][ABQPP-004] - Uri is null")]
        public async Task AttributeBasedQueryParameterProvider_SetParameterAsync_UriIsNull()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new Fixture().Create<PropertyNameTestCommand>();
            var message = new HttpRequestMessage();

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await sut.SetParametersAsync(message, command, default));
        }
    }

    [QueryParameter]
    public record WholeObjectTestCommand : Command
    {
        public string Parameter { get; }

        public WholeObjectTestCommand(string parameter)
        {
            Parameter = parameter;
        }
    }

    public record PropertyTestCommand : Command
    {
        [QueryParameter]
        public string Parameter { get; }

        public PropertyTestCommand(string parameter)
        {
            Parameter = parameter;
        }
    }

    public record PropertyNameTestCommand : Command
    {
        [QueryParameter(Name = "OverridedParameter")]
        public string Parameter { get; }

        public PropertyNameTestCommand(string parameter)
        {
            Parameter = parameter;
        }
    }
}

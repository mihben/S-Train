using AutoFixture;
using STrain.CQS.Attributes.RequestSending.Http.Parameters;
using STrain.CQS.NetCore.RequestSending.Providers.Attributive;
using STrain.CQS.Test.Unit.Supports;

namespace STrain.CQS.Test.Unit.NetCore.RequestSending
{
    public class AttributiveHeaderParameterProviderTest
    {
        private AttributiveHeaderParameterProvider CreateSUT()
        {
            return new AttributiveHeaderParameterProvider();
        }

        [Fact(DisplayName = "[UNIT][AHPP-001] - Set 'Request-Type' header")]
        public async Task AttributiveHeaderParameterProvider_SetParameterAsync_SetHeaderFromAttribute()
        {
            // Arrange
            var sut = CreateSUT();
            var message = new HttpRequestMessage(HttpMethod.Get, "http://test.com");
            var request = new Fixture().Create<TestCommand>();

            // Act
            await sut.SetParametersAsync(message, request, default);

            // Assert
            Assert.Contains(message.Headers, h => h.Key.Equals("request-type") && h.Value.Contains($"{request.GetType().FullName}, {request.GetType().Assembly.GetName().Name}"));
        }

        [Fact(DisplayName = "[UNIT][AHPP-002] - Send Whole Object as Header")]
        public async Task AttributiveHeaderParameterProvider_SetParameterAsync_SendWholeObjectAsHeader()
        {
            // Arrange
            var sut = CreateSUT();
            var message = new HttpRequestMessage(HttpMethod.Get, "http://test.com");
            var request = new Fixture().Create<TestHeaderCommand>();

            // Act
            await sut.SetParametersAsync(message, request, default);

            // Assert
            Assert.Contains(message.Headers, h => h.Key.Equals("Parameter") && h.Value.Contains(request.Parameter));
        }

        [Fact(DisplayName = "[UNIT][AHPP-003] - Set Header from Properties")]
        public async Task AttributiveHeaderParameterProvider_SetParameterAsync_SetHeaderFromProperties()
        {
            // Arrange
            var sut = CreateSUT();
            var message = new HttpRequestMessage(HttpMethod.Get, "http://test.com");
            var request = new Fixture().Create<TestHeaderPropertyCommand>();

            // Act
            await sut.SetParametersAsync(message, request, default);

            // Assert
            Assert.Contains(message.Headers, h => h.Key.Equals("test-parameter") && h.Value.Contains(request.Parameter));
        }
    }

    [Patch("patch")]
    [HeaderParameter]
    internal record TestHeaderCommand : Command
    {
        public string Parameter { get; }

        public TestHeaderCommand(string parameter)
        {
            Parameter = parameter;
        }
    }

    [Patch("patch")]
    internal record TestHeaderPropertyCommand : Command
    {
        [HeaderParameter(Name = "test-parameter")]
        public string Parameter { get; }

        public TestHeaderPropertyCommand(string parameter)
        {
            Parameter = parameter;
        }
    }
}

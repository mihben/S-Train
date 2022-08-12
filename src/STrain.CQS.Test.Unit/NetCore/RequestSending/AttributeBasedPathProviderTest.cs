using AutoFixture;
using Microsoft.Extensions.Options;
using Moq;
using STrain.CQS.NetCore.RequestSending;
using STrain.CQS.Test.Unit.Supports;

namespace STrain.CQS.Test.Unit.NetCore.RequestSending
{
    public class AttributeBasedPathProviderTest
    {
        private Mock<IOptions<HttpRequestSenderOptions>> _optionsMock;

        private AttributeBasedPathProvider CreateSUT()
        {
            _optionsMock = new Mock<IOptions<HttpRequestSenderOptions>>();

            return new AttributeBasedPathProvider(_optionsMock.Object);
        }

        [Fact(DisplayName = "[UNIT][ABPP-001] - Get Path from Attribute")]
        public void AttributeBasedPathProvider_GetPath_GetPathFromAttribute()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new Fixture().Create<TestExternalWithAttributeCommand>();

            // Act
            var result = sut.GetPath(command);

            // Assert
            Assert.Equal($"test-path/{command.Parameter}", result);
        }

        [Fact(DisplayName = "[UNIT][ABPP-002] - Get Generic Handler Path")]
        public void AttributeBasedPathProvider_GetPath_GetGenericHandlerPath()
        {
            // Arrange
            var sut = CreateSUT();
            var options = new Fixture().Create<HttpRequestSenderOptions>();

            _optionsMock.Setup(o => o.Value)
                .Returns(options);

            // Act
            var result = sut.GetPath(new TestExternalWithoutAttributeCommand());

            // Assert
            Assert.Equal(options.Path, result);
        }

        [Fact(DisplayName = "[UNIT][ABPP-003] - Wrong Parameter Reference")]
        public void AttributeBasedPathProvider_GetPath_WrongParameterReference()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            Assert.Throws<InvalidOperationException>(() => sut.GetPath(new TestExternalWithWrongPathParameterCommand()));
        }
    }
}

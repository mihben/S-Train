using AutoFixture;
using Microsoft.Extensions.Options;
using Moq;
using STrain.CQS.NetCore.RequestSending;
using STrain.CQS.NetCore.RequestSending.Attributive;

namespace STrain.CQS.Test.Unit.NetCore.RequestSending
{
    public class AttributivePathProviderTest
    {
        private Mock<IOptions<HttpRequestSenderOptions>> _optionsMock = null!;

        private AttributivePathProvider CreateSUT()
        {
            _optionsMock = new Mock<IOptions<HttpRequestSenderOptions>>();

            return new AttributivePathProvider(_optionsMock.Object);
        }

        [Fact(DisplayName = "[UNIT][ABPP-001] - Get Path from Attribute")]
        public void AttributeBasedPathProvider_GetPath_GetPathFromAttribute()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new Fixture().Create<ExternalCommand>();

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
            var result = sut.GetPath(new GenericCommand());

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
            Assert.Throws<InvalidOperationException>(() => sut.GetPath(new Fixture().Create<InvalidExternalCommand>()));
        }

        [Fact(DisplayName = "[UNIT][ABPP-004] - Path does not configured in configuration")]
        public void AttributeBasedPathProvider_GetPath_PathDoesNotConfiguredInConfiguration()
        {
            // Arrange
            var sut = CreateSUT();

            _optionsMock.Setup(o => o.Value)
                .Returns(new HttpRequestSenderOptions());

            // Act
            // Assert
            Assert.Throws<InvalidOperationException>(() => sut.GetPath(new Fixture().Create<GenericCommand>()));
        }
    }

    [Patch("test-path/{parameter}")]
    internal record ExternalCommand : Command
    {
        public string Parameter { get; }

        public ExternalCommand(string parameter)
        {
            Parameter = parameter;
        }
    }

    [Patch("test-path/{wrongparameter}")]
    internal record InvalidExternalCommand : Command
    {
        public string Parameter { get; }

        public InvalidExternalCommand(string parameter)
        {
            Parameter = parameter;
        }
    }

    internal record GenericCommand : Command { }
}

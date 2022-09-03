using AutoFixture;
using Microsoft.Extensions.Logging;
using STrain.CQS.NetCore.RequestSending.Attributive;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Unit.NetCore.RequestSending
{
    public class AttributivePathProviderTest
    {
        private readonly ILogger<AttributivePathProvider> _logger;

        public AttributivePathProviderTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                            .AddXUnit(outputHelper)
                            .CreateLogger<AttributivePathProvider>();
        }

        private AttributivePathProvider CreateSUT(string? path = null)
        {
            return new AttributivePathProvider(path, _logger);
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
            var path = new Fixture().Create<string>();
            var sut = CreateSUT(path);

            // Act
            var result = sut.GetPath(new GenericCommand());

            // Assert
            Assert.Equal(path, result);
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

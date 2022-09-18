using AutoFixture;
using Microsoft.Extensions.Logging;
using STrain.CQS.Http.RequestSending.Binders.Generic;
using STrain.CQS.Test.Unit.Supports;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Unit.Http
{
    public class GenericBodyParameterBinderTest
    {
        private readonly ILogger<GenericBodyParameterBinder> _logger;

        public GenericBodyParameterBinderTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                            .AddXUnit(outputHelper)
                            .CreateLogger<GenericBodyParameterBinder>();
        }

        private GenericBodyParameterBinder CreateSUT()
        {
            return new GenericBodyParameterBinder(_logger);
        }

        [Fact(DisplayName = "[UNIT][GBPB-001] - Bind command")]
        public async Task GenericBodyParameterBinder_BindAsync_BindCommand()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new Fixture().Create<TestCommand>();

            // Act
            var result = await sut.BindAsync(command, default);

            // Assert
            var content = Assert.IsType<JsonContent>(result);
            Assert.Equal(command, content.Value);
        }

        [Fact(DisplayName = "[UNIT][GBPB-002] - Request is null")]
        public async Task GenericBodyParameterBinder_BindAsync_RequestIsNull()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.BindAsync<TestCommand>(null, default));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact(DisplayName = "[UNIT][GBPB-003] - Bind query")]
        public async Task GenericBodyParameterBinder_BindAsync_BindQuery()
        {
            // Arrange
            var sut = CreateSUT();
            var query = new Fixture().Create<TestQuery>();

            // Act
            var result = await sut.BindAsync(query, default);

            // Assert
            Assert.Null(result);
        }
    }
}

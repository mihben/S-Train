using AutoFixture;
using STrain.CQS.Http.RequestSending.Binders.Generic;
using STrain.CQS.Test.Unit.Supports;

namespace STrain.CQS.Test.Unit.Http.RequestSending
{
    public class GenericHeaderParameterBinderTest
    {
#pragma warning disable CA1822 // Mark members as static
        private GenericHeaderParameterBinder CreateSUT()
#pragma warning restore CA1822 // Mark members as static
        {
            return new GenericHeaderParameterBinder();
        }

        [Fact(DisplayName = "[UNIT][GHPB-001] - Bind 'Request-Type'")]
        public async Task GenericHeaderParameterBinder_BindAsync_BindRequestType()
        {
            // Arrange
            var command = new Fixture().Create<TestCommand>();
            var sut = CreateSUT();
            var message = new HttpRequestMessage();

            // Act
            await sut.BindAsync(command, message.Headers, default);

            // Assert
            Assert.Contains(message.Headers, h => h.Key.Equals("Request-Type") && h.Value.First().Equals("STrain.CQS.Test.Unit.Supports.TestCommand, STrain.CQS.Test.Unit"));
        }
    }
}

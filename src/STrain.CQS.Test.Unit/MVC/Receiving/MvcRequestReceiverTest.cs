using Microsoft.AspNetCore.Mvc;
using Moq;
using STrain.CQS.Dispatchers;
using STrain.CQS.MVC.Receiving;

namespace STrain.CQS.Test.Unit.MVC.Receiving
{
    public class MvcRequestReceiverTest
    {
        private Mock<ICommandDispatcher> _commandDispatcherMock = null!;
        private Mock<IQueryDispatcher> _queryDispatcherMock = null!;

        private MvcRequestReceiver CreateSUT()
        {
            _commandDispatcherMock = new Mock<ICommandDispatcher>();
            _queryDispatcherMock = new Mock<IQueryDispatcher>();

            return new MvcRequestReceiver(_commandDispatcherMock.Object, _queryDispatcherMock.Object);
        }

        [Fact(DisplayName = "[UNIT][MRR-001] - Receiving command")]
        public async Task MvcRequestReceiver_ReceiveAsync_ReceivingCommand()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            var response = await sut.ReceiveAsync(new TestCommand(), default);

            // Assert
            Assert.IsType<AcceptedResult>(response);
        }

        [Fact(DisplayName = "[UNIT][MRR-002] - Receiving query")]
        public async Task MvcRequestReceiver_ReceiveAsync_ReceivingQuery()
        {
            // Arrange
            var sut = CreateSUT();
            var result = new object();

            _queryDispatcherMock.Setup(qd => qd.DispatchAsync(It.IsAny<TestQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await sut.ReceiveAsync<TestQuery, object>(new TestQuery(), default);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(result, okObjectResult.Value);
        }

        private record TestCommand : Command { }
        private record TestQuery : Query<object> { }
    }
}

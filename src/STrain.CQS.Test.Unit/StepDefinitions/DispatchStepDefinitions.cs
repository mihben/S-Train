using Moq;
using STrain.CQS.Dispatchers;
using STrain.CQS.Test.Unit.Support;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Unit.StepDefinitions
{
    [Binding]
    public class DispatchStepDefinitions
    {
        private readonly RequestDispatcher _sut;

        private readonly Mock<IServiceProvider> _serviceProviderMock = new();
        private readonly Mock<ICommandPerformer<TestCommand>> _commandPerformerMock = new();
        private readonly Mock<IQueryPerformer<TestQuery, object>> _queryPerformerMock = new();
        private readonly ExceptionContext _exceptionContext;

        private TestCommand? _command;
        private TestQuery? _query;

        public DispatchStepDefinitions(ITestOutputHelper outputHelper, ExceptionContext exceptionContext)
        {
            _sut = new RequestDispatcher(_serviceProviderMock.Object, TestLoggerFactory.CreateLogger<RequestDispatcher>(outputHelper));
            _exceptionContext = exceptionContext;
        }


        [Given("Performer registrered for command")]
        public void RegistrateCommandPerformer()
        {
            _serviceProviderMock.Setup(sp => sp.GetService(typeof(IEnumerable<ICommandPerformer<TestCommand>>)))
                .Returns(new List<ICommandPerformer<TestCommand>> { _commandPerformerMock.Object });
        }


        [Given("Performer registrered for query")]
        public void RegistrateQueryPerformer()
        {
            _serviceProviderMock.Setup(sp => sp.GetService(typeof(IEnumerable<IQueryPerformer<TestQuery, object>>)))
                .Returns(new List<IQueryPerformer<TestQuery, object>> { _queryPerformerMock.Object });
        }

        [Given("Multiple performer registered for command")]
        public void RegistrateMultipleCommandPerformer()
        {
            _serviceProviderMock.Setup(sp => sp.GetService(typeof(IEnumerable<ICommandPerformer<TestCommand>>)))
                .Returns(new List<ICommandPerformer<TestCommand>> { _commandPerformerMock.Object, _commandPerformerMock.Object });
        }

        [Given("Multiple performer registered for query")]
        public void RegistrateMultipleQueryPerformer()
        {
            _serviceProviderMock.Setup(sp => sp.GetService(typeof(IEnumerable<IQueryPerformer<TestQuery, object>>)))
                .Returns(new List<IQueryPerformer<TestQuery, object>> { _queryPerformerMock.Object, _queryPerformerMock.Object });
        }

        [When("Dispatching command")]
        public async Task DispatchingCommandAsync()
        {
            _command = new TestCommand();
            try
            {
                await _sut.DispatchAsync(_command, default);
            }
            catch (Exception ex)
            {
                _exceptionContext.Exception = ex;
            }
        }

        [When("Dispatching query")]
        public async Task DispatchingQueryAsync()
        {
            _query = new TestQuery();
            try
            {
                await _sut.DispatchAsync<TestQuery, object>(_query, default);
            }
            catch (Exception ex)
            {
                _exceptionContext.Exception = ex;
            }
        }

        [Then("Command performer should be performed")]
        public void ShouldBePerformedCommandPerformer()
        {
            Assert.NotNull(_command);
            _commandPerformerMock.Verify(cp => cp.PerformAsync(_command, It.IsAny<CancellationToken>()));
        }

        [Then("Query performer should be performed")]
        public void ShouldBePerformedQueryPerformer()
        {
            Assert.NotNull(_query);
            _queryPerformerMock.Verify(cp => cp.PerformAsync(_query, It.IsAny<CancellationToken>()));
        }
    }
}
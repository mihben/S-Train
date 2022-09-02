using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using STrain.CQS.Dispatchers;
using STrain.CQS.Test.Function.Drivers;
using STrain.CQS.Test.Function.Support;
using STrain.CQS.Test.Function.Workarounds;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Function.StepDefinitions
{
    [Binding]
    public class DispatchStepDefinitions
    {
        private readonly CommandDispatcher _commandDispatcher;
        private readonly QueryDispatcher _queryDispatcher;

        private readonly Mock<IServiceProvider> _serviceProviderMock = new();
        private readonly Mock<ICommandPerformer<TestCommand>> _commandPerformerMock = new();
        private readonly Mock<IQueryPerformer<TestQuery, string>> _queryPerformerMock = new();
        private readonly RequestContext _requestContext;
        private readonly ExceptionContext _exceptionContext;
        private readonly WebApplicationFactory<Program> _driver;
        private TestCommand? _command;
        private TestQuery? _query;

        public DispatchStepDefinitions(ITestOutputHelper outputHelper, RequestContext requestContext, ExceptionContext exceptionContext)
        {
            _commandDispatcher = new CommandDispatcher(_serviceProviderMock.Object, TestLoggerFactory.CreateLogger<CommandDispatcher>(outputHelper));
            _queryDispatcher = new QueryDispatcher(_serviceProviderMock.Object, TestLoggerFactory.CreateLogger<QueryDispatcher>(outputHelper));
            _requestContext = requestContext;
            _exceptionContext = exceptionContext;

            _driver = new LightinjectWebApplicationFactory<Program>()
                        .Initialize(outputHelper)
                        .MockAuthentication(builder => builder.Authorized()
                                                                .Unathorized()
                                                                .Forbidden());
        }


        [Given("Performer registrered for command")]
        public void RegistrateCommandPerformer()
        {
            _serviceProviderMock.Setup(sp => sp.GetService(typeof(ICommandPerformer<TestCommand>)))
                .Returns(_commandPerformerMock.Object);
        }


        [Given("Performer registrered for query")]
        public void RegistrateQueryPerformer()
        {
            _serviceProviderMock.Setup(sp => sp.GetService(typeof(IQueryPerformer<TestQuery, string>)))
                .Returns(_queryPerformerMock.Object);
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
            _serviceProviderMock.Setup(sp => sp.GetService(typeof(IEnumerable<IQueryPerformer<TestQuery, string>>)))
                .Returns(new List<IQueryPerformer<TestQuery, string>> { _queryPerformerMock.Object, _queryPerformerMock.Object });
        }

        [When("Dispatching command")]
        public async Task DispatchingCommandAsync()
        {
            _command = new TestCommand();
            try
            {
                await _commandDispatcher.DispatchAsync(_command, default);
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
                await _queryDispatcher.DispatchAsync(_query, default);
            }
            catch (Exception ex)
            {
                _exceptionContext.Exception = ex;
            }
        }

        [When("Dispatching authorized request")]
        public async Task DispatchingAuthorizedRequest()
        {
            _requestContext.Response = await _driver.SendAsync("/api/Authorization/authorized", TimeSpan.FromSeconds(1));
        }

        [When("Dispatching unauthorized request")]
        public async Task DispatchingUnauthorizedRequest()
        {
            _requestContext.Response = await _driver.SendAsync("/api/Authorization/unauthorized", TimeSpan.FromSeconds(1));
        }

        [When("Dispatching forbidden request")]
        public async Task DispatchingForbiddenRequest()
        {
            _requestContext.Response = await _driver.SendAsync("/api/Authorization/forbidden", TimeSpan.FromSeconds(1));
        }

        [When("Dispatching allow anonymus request")]
        public async Task DispatchingAllowAnonymisRequest()
        {
            _requestContext.Response = await _driver.SendAsync("/api/Authorization/allow-anonymus", TimeSpan.FromSeconds(1));
        }

        [Then("Command performer should be performed")]
        public void ShouldBePerformedCommandPerformer()
        {
            Assert.NotNull(_command);
            _commandPerformerMock.Verify(cp => cp.PerformAsync(_command!, It.IsAny<CancellationToken>()));
        }

        [Then("Query performer should be performed")]
        public void ShouldBePerformedQueryPerformer()
        {
            Assert.NotNull(_query);
            _queryPerformerMock.Verify(cp => cp.PerformAsync(_query!, It.IsAny<CancellationToken>()));
        }
    }
}
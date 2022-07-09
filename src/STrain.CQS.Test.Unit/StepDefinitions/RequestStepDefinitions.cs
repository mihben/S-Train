using STrain.CQS.Test.Unit.Support;

namespace STrain.CQS.Test.Unit.StepDefinitions
{
    [Binding]
    public class RequestStepDefinitions
    {
        private IRequest? _a;
        private IRequest? _b;
        private bool? _equals = null;

        [Given("Command with {Guid} id")]
        public void CreateCommand(Guid requestId)
        {
            if (_a is null) _a = new TestCommand(requestId);
            else _b = new TestCommand(requestId);
        }

        [Given("Query with {Guid} id")]
        public void CreateQuery(Guid requestId)
        {
            if (_a is null) _a = new TestQuery(requestId);
            else _b = new TestQuery(requestId);
        }

        [Given("{string} query is null")]
        public void QueryIsNull(string query)
        {
            if (query.Equals("A")) _a = null;
            if (query.Equals("B")) _b = null;
        }

        [Given("{string} command is null")]
        public void CommandIsNull(string command)
        {
            if (command.Equals("A")) _a = null;
            if (command.Equals("B")) _b = null;
        }

        [When("Comparing requests")]
        public void CompareRequests()
        {
            if (_a is null) throw new InvalidOperationException("Request is null");
            _equals = _a.Equals(_b);
        }

        [When("Creating command")]
        public void CreateCommand()
        {
            _a = new TestCommand();
        }

        [When("Creating query")]
        public void CreateQuery()
        {
            _a = new TestQuery();
        }

        [Then("Should be equals")]
        public void ShouldBeEquals()
        {
            Assert.True(_equals);
        }

        [Then("Should not be equals")]
        public void ShouldNotBeEquals()
        {
            Assert.False(_equals);
        }

        [Then("Id should be generated")]
        public void ShouldGenerateId()
        {
            Assert.NotEqual(Guid.Empty, _a.RequestId);
        }
    }
}

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


        [When("Comparing requests")]
        public void CompareRequests()
        {
            _equals = _a.Equals(_b);
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
    }
}

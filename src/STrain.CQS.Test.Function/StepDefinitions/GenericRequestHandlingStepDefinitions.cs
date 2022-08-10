using Moq;
using STrain.CQS.Test.Function.Drivers;
using STrain.Sample.Api;
using System.Net;

namespace STrain.CQS.Test.Function.StepDefinitions
{
    [Binding]
    public class GenericRequestHandlingStepDefinitions
    {
        private readonly SampleApiDriver _driver;

        private SampleCommand? _command;
        private HttpResponseMessage _response;

        public GenericRequestHandlingStepDefinitions(SampleApiDriver driver)
        {
            _driver = driver;
        }


        [When("Receiving command")]
        public async Task ReceiveCommandAsync()
        {
            _command = new SampleCommand();
            _response = await _driver.SendCommandAsync(_command, TimeSpan.FromSeconds(5));
        }


        [Then("Command should be performed by performer")]
        public void CommandShouldBePerformedByPerformer()
        {
            Assert.Equal(HttpStatusCode.Accepted, _response.StatusCode);
            _driver.CommandPerformerMock.Verify(cp => cp.PerformAsync(_command, It.IsAny<CancellationToken>()));
        }
    }
}

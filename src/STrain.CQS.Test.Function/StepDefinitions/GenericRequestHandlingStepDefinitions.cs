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


        [Then("Response status code should be {string}")]
        public void ResponseStatusCodeShouldBe(string statusCode)
        {
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode), _response.StatusCode);
        }
    }
}

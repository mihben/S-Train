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
        private SampleQuery? _query;
        private HttpResponseMessage? _response;

        public GenericRequestHandlingStepDefinitions(SampleApiDriver driver)
        {
            _driver = driver;
        }

        [When("Receiving command")]
        public async Task ReceiveCommandAsync()
        {
            _command = new SampleCommand();
            _response = await _driver.SendCommandAsync(_command, TimeSpan.FromSeconds(1));
        }
        

        [When("Receiving query")]
        public async Task ReceiveQueryAsync()
        {
            _query = new SampleQuery();
            _response = await _driver.SendQueryAsync<SampleQuery, string>(_query, TimeSpan.FromSeconds(1));
        }

        [Then("Response should be")]
        public async Task ResponseShouldBeAsync(Table dataTable)
        {
            Assert.Equal(Enum.Parse<HttpStatusCode>(dataTable.Rows[0]["StatusCode"]), _response.StatusCode);
            Assert.Equal(dataTable.Rows[0]["Content"], await _response.Content.ReadAsStringAsync());
        }
    }
}

using AutoFixture;
using STrain.CQS.Test.Function.Drivers;
using System.Net;

namespace STrain.CQS.Test.Function.StepDefinitions
{
    [Binding]
    public class GenericRequestHandlingStepDefinitions
    {
        private readonly ApiDriver _apiDriver;
        private HttpResponseMessage _response = null!;

        public GenericRequestHandlingStepDefinitions(ApiDriver apiDriver)
        {
            _apiDriver = apiDriver;
        }

        [When("Receiving command via generic controller")]
        public async Task ReceivingCommandAsync()
        {
            _response = await _apiDriver.ReceiveAsync(new Fixture().Create<Sample.Api.Sample.GenericCommand>(), TimeSpan.FromSeconds(1));
        }

        [When("Receiving query via generic controller")]
        public async Task ReceivingQueryAsync()
        {
            _response = await _apiDriver.ReceiveAsync<Sample.Api.Sample.GenericQuery, string>(new Fixture().Create<Sample.Api.Sample.GenericQuery>(), TimeSpan.FromSeconds(1));
        }

        [Then("Response should be")]
        public async Task ShouldResponseAsync(Table dataTable)
        {
            Assert.Equal(dataTable.GetEnum<HttpStatusCode>("StatusCode"), _response.StatusCode);
            Assert.Equal(dataTable.GetValue<string>("Content"), await _response.Content.ReadAsStringAsync());
        }
    }
}

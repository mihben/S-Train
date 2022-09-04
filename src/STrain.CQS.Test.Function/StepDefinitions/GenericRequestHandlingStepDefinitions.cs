using AutoFixture;
using Microsoft.AspNetCore.Mvc.Testing;
using STrain.CQS.Test.Function.Drivers;
using STrain.CQS.Test.Function.Support;
using System.Net;

namespace STrain.CQS.Test.Function.StepDefinitions
{
    [Binding]
    internal class GenericRequestHandlingStepDefinitions
    {
        private readonly WebApplicationFactory<Program> _driver;
        private readonly RequestContext _requestContext;
        private Sample.Api.Sample.GenericCommand? _command;
        private readonly Sample.Api.Sample.GenericQuery? _query;

        public GenericRequestHandlingStepDefinitions(RequestContext requestContext)
        {
            _driver = new WebApplicationFactory<Program>();
            _requestContext = requestContext;
        }

        [When("Receiving command")]
        public async Task ReceiveCommandAsync()
        {
            _command = new Fixture().Create<Sample.Api.Sample.GenericCommand>();
            _requestContext.Response = await _driver.ReceiveCommandAsync(_command, TimeSpan.FromSeconds(1));
        }


        [When("Receiving query")]
        public async Task ReceiveQueryAsync()
        {
            //_query = new SampleQuery(new Fixture().Create<string>());
            //_requestContext.Response = await _driver.ReceiveQueryAsync<SampleQuery, SampleQuery.Result>(_query, TimeSpan.FromSeconds(1));
        }

        [Then("Response should be")]
        public async Task ResponseShouldBeAsync(Table dataTable)
        {
            Assert.NotNull(_requestContext.Response);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.Equal(Enum.Parse<HttpStatusCode>(dataTable.Rows[0]["StatusCode"]), _requestContext.Response.StatusCode);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            if (dataTable.Rows[0].ContainsKey("Content")) Assert.Equal(dataTable.Rows[0]["Content"], await _requestContext.Response.Content.ReadAsStringAsync());
        }
    }
}

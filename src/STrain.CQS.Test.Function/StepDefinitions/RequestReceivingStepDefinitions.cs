using AutoFixture;
using LightInject;
using Microsoft.AspNetCore.TestHost;
using Moq;
using STrain.CQS.Test.Function.Drivers;
using System.Net;

namespace STrain.CQS.Test.Function.StepDefinitions
{
    [Binding]
    public class RequestReceivingStepDefinitions
    {
        private readonly ApiDriver _apiDriver;

        private Mock<ICommandPerformer<Sample.Api.Sample.GenericCommand>> _commandPerformerMock = null!;
        private Sample.Api.Sample.GenericCommand _command = null!;
        private Sample.Api.Sample.GenericQuery _query = null!;
        private HttpResponseMessage _response = null!;
        private Mock<IQueryPerformer<Sample.Api.Sample.GenericQuery, string>> _queryPerformerMock = null!;

        public RequestReceivingStepDefinitions(ApiDriver apiDriver)
        {
            _apiDriver = apiDriver;
        }

        [Given("Registered command performer")]
        public void MockCommandPerformer()
        {
            _commandPerformerMock = new Mock<ICommandPerformer<Sample.Api.Sample.GenericCommand>>();
            _apiDriver.WithWebHostBuilder(builder => builder.ConfigureTestContainer<IServiceContainer>(container => container.Override(registration => registration.ServiceType.Equals(typeof(ICommandPerformer<Sample.Api.Sample.GenericCommand>)), (_, registration) =>
            {
                registration.Value = _commandPerformerMock.Object;
                return registration;
            })));
        }

        [Given("Registered query performer")]
        public void MockQueryPerformer()
        {
            _queryPerformerMock = new Mock<IQueryPerformer<Sample.Api.Sample.GenericQuery, string>>();
            _apiDriver.WithWebHostBuilder(builder => builder.ConfigureTestContainer<IServiceContainer>(container => container.Override(registration => registration.ServiceType.Equals(typeof(IQueryPerformer<Sample.Api.Sample.GenericQuery, string>)), (_, registration) =>
            {
                registration.Value = _queryPerformerMock.Object;
                return registration;
            })));
        }

        [When("Receiving command")]
        public async Task ReceivingCommandAsync()
        {
            _command = new Fixture().Create<Sample.Api.Sample.GenericCommand>();
            await _apiDriver.ReceiveViaReceierAsync(_command, TimeSpan.FromSeconds(1));
        }

        [When("Receiving query")]
        public async Task ReceivingQueryAsync()
        {
            _query = new Fixture().Create<Sample.Api.Sample.GenericQuery>();
            await _apiDriver.ReceiveViaReceierAsync<Sample.Api.Sample.GenericQuery, string>(_query, TimeSpan.FromSeconds(1));
        }

        [When("Receiving authorized request")]
        public async Task ReceivingAuthorizedRequestAsync()
        {
            _response = await _apiDriver.SendAsync("/api/authorization/authorized", TimeSpan.FromSeconds(1));
        }

        [When("Receiving unauthorized request")]
        public async Task ReceivingUnauthorizedRequestAsync()
        {
            _response = await _apiDriver.SendAsync("/api/authorization/unauthorized", TimeSpan.FromSeconds(1));
        }

        [When("Receiving forbidden request")]
        public async Task ReceivingForbiddenRequestAsync()
        {
            _response = await _apiDriver.SendAsync("/api/authorization/forbidden", TimeSpan.FromSeconds(1));
        }

        [When("Receiving allow anonymus request")]
        public async Task ReceivingAllowAnonymisRequestAsync()
        {
            _response = await _apiDriver.SendAsync("/api/authorization/allow-anonymus", TimeSpan.FromSeconds(1));
        }

        [Then("Performer should be called")]
        public void ShouldBeCalledPerformer()
        {
            if (_commandPerformerMock is not null) _commandPerformerMock.Verify(cp => cp.PerformAsync(_command, It.IsAny<CancellationToken>()));
            if (_queryPerformerMock is not null) _queryPerformerMock.Verify(cp => cp.PerformAsync(_query, It.IsAny<CancellationToken>()));
        }

        [Then("Authorization response should be")]
        public void ShouldBeResponde(Table dataTable)
        {
            Assert.Equal(dataTable.GetEnum<HttpStatusCode>("StatusCode"), _response.StatusCode);
        }
    }
}

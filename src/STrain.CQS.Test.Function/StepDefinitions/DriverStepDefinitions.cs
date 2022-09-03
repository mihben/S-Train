using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using STrain.CQS.Test.Function.Drivers;
using STrain.Sample.Api;
using System.Security.Cryptography;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Function.StepDefinitions
{
    [Binding]
    public class DriverStepDefinitions
    {
        private readonly ApiDriver _apiDriver;
        private Mock<HttpMessageHandler> _messageHandlerMock;

        public DriverStepDefinitions(ApiDriver apiDriver)
        {
            _apiDriver = apiDriver;
        }

        [When("Testing")]
        public async Task TestingAsync()
        {
            _messageHandlerMock = _apiDriver.MockHttpSender("internal", "/api", "http://localhost:5100/");
            await _apiDriver.SendAsync(new SampleCommand("Teszt"), TimeSpan.FromSeconds(5));
        }

        [Then("Successfull")]
        public void Successfull()
        {
            
        }
    }
}

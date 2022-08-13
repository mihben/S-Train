﻿using AutoFixture;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using Moq.Protected;
using STrain.CQS.Test.Function.Drivers;
using STrain.Sample.Api;
using System.Text.Json;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Function.StepDefinitions
{
    [Binding]
    internal class HttpRequestSendingStepDefinitions
    {
        private readonly Mock<HttpMessageHandler> _messageHandlerMock = new();
        private readonly string _baseAddress = "http://strain-service/";
        private readonly string _path = "api";

        private readonly WebApplicationFactory<Program> _driver;
        private SampleCommand _command;
        private SampleQuery _query;

        public HttpRequestSendingStepDefinitions(ITestOutputHelper outputHelper)
        {
            _messageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK));

            _driver = new WebApplicationFactory<Program>()
                        .Initialize(outputHelper)
                        .MockHttpSender(_messageHandlerMock.Object, _path, _baseAddress);
        }


        [When("Sending command to STrain service")]
        public async Task SendingCommandToStrainServiceAsync()
        {
            _command = new Fixture().Create<SampleCommand>();
            await _driver.SendCommandAsync(_command, TimeSpan.FromSeconds(1));
        }


        [When("Sending query to STrain service")]
        public async Task SendingQueryToStrainServiceAsync()
        {
            _query = new Fixture().Create<SampleQuery>();
            await _driver.SendQueryAsync<SampleQuery, SampleQuery.Result>(_query, TimeSpan.FromSeconds(1));
        }

        [Then("Request should be sent")]
        public void ShouldSentRequest(Table dataTable)
        {
            var uri = new Uri($"{dataTable.Rows[0]["BaseAddress"]}{dataTable.Rows[0]["Path"]}");
            if (_command is not null) _messageHandlerMock.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(m => m.Verify(dataTable.Rows[0]["Method"], uri, _command)), ItExpr.IsAny<CancellationToken>());
            if (_query is not null) _messageHandlerMock.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(m => m.Verify(dataTable.Rows[0]["Method"], uri, _query)), ItExpr.IsAny<CancellationToken>());
        }
    }

    internal static class HttpRequestSendingStepDefinitionsExtensions
    {
        public static bool Verify<TRequest>(this HttpRequestMessage message, string method, Uri uri, TRequest request)
            where TRequest : IRequest
        {
            return message.Method.Method.Equals(method)
                && message.RequestUri.AbsoluteUri.Equals(uri.AbsoluteUri)
                && message.Headers.GetValues("request-type").First().Equals($"{request.GetType().FullName}, {request.GetType().Assembly.GetName().Name}")
                && JsonSerializer.Deserialize<TRequest>(message.Content.ReadAsStream(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).Equals(request);
        }
    }
}

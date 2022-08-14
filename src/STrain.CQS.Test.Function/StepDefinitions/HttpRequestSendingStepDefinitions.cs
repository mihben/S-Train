﻿using AutoFixture;
using Logproto;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using Moq.Protected;
using STrain.CQS.Attributes.RequestSending.Http.Parameters;
using STrain.CQS.Test.Function.Drivers;
using STrain.Sample.Api;
using System.Text.Json;
using Xunit.Abstractions;
using static System.Net.WebRequestMethods;

namespace STrain.CQS.Test.Function.StepDefinitions
{
    [Binding]
    internal class HttpRequestSendingStepDefinitions
    {
        private readonly Mock<HttpMessageHandler> _messageHandlerMock = new();
        private string _baseAddress = "http://test-service";
        private readonly string _path = "api";

        private readonly WebApplicationFactory<Program> _driver;
        private SampleCommand _command;
        private SampleQuery _query;
        private GetRequest _getRequest;
        private PostRequest _postRequest;

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

        [When("Sending {string} request to external endpoint")]
        public async Task SendindRequestToExternalServiceAsync(string method)
        {
            switch (method)
            {
                case "GET":
                    _getRequest = new Fixture().Create<GetRequest>();
                    await _driver.SendRequestAsync<GetRequest, string>(_getRequest, TimeSpan.FromSeconds(1));
                    break;
                case "POST":
                    _postRequest = new Fixture().Create<PostRequest>();
                    await _driver.SendRequestAsync<PostRequest, string>(_postRequest, TimeSpan.FromSeconds(1));
                    break;
                default:
                    throw new InvalidOperationException($"{method} method is unsupported");
            }
        }

        [Then("Request should be sent")]
        public void ShouldSentRequest(Table dataTable)
        {
            var uri = new Uri($"{dataTable.Rows[0]["BaseAddress"]}{dataTable.Rows[0]["Path"]}");
            if (_command is not null) _messageHandlerMock.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(m => m.VerifyGenericRequest(dataTable.Rows[0]["Method"], uri, _command)), ItExpr.IsAny<CancellationToken>());
            if (_query is not null) _messageHandlerMock.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(m => m.VerifyGenericRequest(dataTable.Rows[0]["Method"], uri, _query)), ItExpr.IsAny<CancellationToken>());
        }

        [Then("Request should be sent to external endpoint")]
        public void ShouldSentRequestToExternalEndpoint(Table dataTable)
        {
            var method = dataTable.Rows[0]["Method"];
            var baseAddress = dataTable.Rows[0]["BaseAddress"];
            var path = dataTable.Rows[0]["Path"];

            switch (method)
            {
                case "GET":
                    Assert.NotNull(_getRequest);
                    _messageHandlerMock.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(hrm => hrm.VerifyExternalRequest(method, new Uri($"{baseAddress}{path}"), _getRequest.HeaderParameter, $"?query-parameter={_getRequest.QueryParameter}", $"{{\"{nameof(GetRequest.BodyParameter)}\":\"{_getRequest.BodyParameter}\"}}")), ItExpr.IsAny<CancellationToken>());
                    break;
                case "POST":
                    Assert.NotNull(_postRequest);
                    _messageHandlerMock.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(hrm => hrm.VerifyExternalRequest(method, new Uri($"{baseAddress}{path}"), _postRequest.HeaderParameter, $"?query-parameter={_postRequest.QueryParameter}", $"{{\"{nameof(PostRequest.BodyParameter)}\":\"{_postRequest.BodyParameter}\"}}")), ItExpr.IsAny<CancellationToken>());
                    break;
                default:
                    throw new InvalidOperationException($"{method} method is unsupported");
            }
        }
    }

    internal static class HttpRequestSendingStepDefinitionsExtensions
    {
        public static bool VerifyExternalRequest(this HttpRequestMessage message, string method, Uri uri, string header, string query, string body)
        {
            return message.Method.Method.Equals(method)
                && message.RequestUri.AbsoluteUri.StartsWith(uri.AbsoluteUri)
                && message.Headers.GetValues("header-parameter").Contains(header)
                && message.RequestUri.Query.Equals(query)
                && message.Content.ReadAsStringAsync().Result.Equals(body);
        }

        public static bool VerifyGenericRequest<TRequest>(this HttpRequestMessage message, string method, Uri uri, TRequest request)
            where TRequest : IRequest
        {
            return message.Method.Method.Equals(method)
                && message.RequestUri.AbsoluteUri.Equals(uri.AbsoluteUri)
                && message.Headers.GetValues("request-type").First().Equals($"{request.GetType().FullName}, {request.GetType().Assembly.GetName().Name}")
                && JsonSerializer.Deserialize<TRequest>(message.Content.ReadAsStream(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).Equals(request);
        }
    }

    [Get("get-endpoint/{id}")]
    internal record GetRequest : IRequest
    {
        public int Id => 1;

        [HeaderParameter(Name = "header-parameter")]
        public string HeaderParameter { get; }

        [QueryParameter(Name = "query-parameter")]
        public string QueryParameter { get; }
        [BodyParameter]
        public string BodyParameter { get; }

        public GetRequest(string headerParameter, string queryParameter, string bodyParameter)
        {
            HeaderParameter = headerParameter;
            QueryParameter = queryParameter;
            BodyParameter = bodyParameter;
        }
    }

    [Post("post-endpoint/{id}")]
    internal record PostRequest : IRequest
    {
        public int Id => 2;

        [HeaderParameter(Name = "header-parameter")]
        public string HeaderParameter { get; }

        [QueryParameter(Name = "query-parameter")]
        public string QueryParameter { get; }
        [BodyParameter]
        public string BodyParameter { get; }

        public PostRequest(string headerParameter, string queryParameter, string bodyParameter)
        {
            HeaderParameter = headerParameter;
            QueryParameter = queryParameter;
            BodyParameter = bodyParameter;
        }
    }
}
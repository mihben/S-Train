using AutoFixture;
using LightInject;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.WebUtilities;
using Moq;
using STrain.Core.Exceptions;
using STrain.CQS.Http.RequestSending;
using STrain.CQS.Test.Function.Drivers;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace STrain.CQS.Test.Function.StepDefinitions
{
    [Binding]
    public class RequestSendingStepDefinitions
    {
        private readonly ApiDriver _apiDriver;
        private Mock<HttpMessageHandler> _messageHandlerMock = null!;
        private Sample.Api.Sample.GenericCommand _command = null!;
        private Sample.Api.Sample.GenericQuery _query = null!;

        private GetRequest _getRequest = null!;
        private PostRequest _postRequest = null!;
        private PutRequest _putRequest = null!;
        private PatchRequest _patchRequest = null!;
        private DeleteRequest _deleteRequest = null!;

        public RequestSendingStepDefinitions(ApiDriver apiDriver)
        {
            _apiDriver = apiDriver;
        }


        [Given("Configured HTTP sender")]
        public void ConfigureHTTPSender(Table dataTable)
        {
            var key = dataTable.GetValue<string>("Key")!;
            var baseAddress = dataTable.GetValue<string>("BaseAddress")!;
            var path = dataTable.GetValue<string>("Path");

            _apiDriver.SetConfiguration($"Senders:{key}:BaseAddress", baseAddress);
            if (path is not null) _apiDriver.SetConfiguration($"Senders:{key}:Path", path);
            _messageHandlerMock = _apiDriver.MockHttpSender(key, baseAddress);
            _messageHandlerMock.SetupSend();

            Func<IRequest, string> keySelector = _ => key;
            _apiDriver.WithWebHostBuilder(builder => builder.ConfigureTestContainer<IServiceContainer>(registry => registry.Override(registration => registration.ServiceType.Equals(typeof(Func<IRequest, string>)), (_, registration) =>
            {
                registration.Value = keySelector; return registration;
            })));
        }

        [When("Sending generic command")]
        public async Task SendingGenericCommandAsync()
        {
            _command = new Fixture().Create<Sample.Api.Sample.GenericCommand>();
            await _apiDriver.SendAsync(_command, TimeSpan.FromSeconds(1));
        }

        [When("Sending generic query")]
        public async Task SendingGenericQueryAsync()
        {
            _query = new Fixture().Create<Sample.Api.Sample.GenericQuery>();
            await _apiDriver.GetAsync<Sample.Api.Sample.GenericQuery, string>(_query, TimeSpan.FromSeconds(1));
        }


        [When("Sending external {string} request")]
        public async Task SendingExternalRequestAsync(string method)
        {
            switch (method)
            {
                case "GET":
                    _getRequest = new Fixture().Create<GetRequest>();
                    await _apiDriver.SendAsync<GetRequest, object>(_getRequest, TimeSpan.FromSeconds(1));
                    break;
                case "POST":
                    _postRequest = new Fixture().Create<PostRequest>();
                    await _apiDriver.SendAsync<PostRequest, object>(_postRequest, TimeSpan.FromSeconds(1));
                    break;
                case "PUT":
                    _putRequest = new Fixture().Create<PutRequest>();
                    await _apiDriver.SendAsync<PutRequest, object>(_putRequest, TimeSpan.FromSeconds(1));
                    break;
                case "PATCH":
                    _patchRequest = new Fixture().Create<PatchRequest>();
                    await _apiDriver.SendAsync<PatchRequest, object>(_patchRequest, TimeSpan.FromSeconds(1));
                    break;
                case "DELETE":
                    _deleteRequest = new Fixture().Create<DeleteRequest>();
                    await _apiDriver.SendAsync<DeleteRequest, object>(_deleteRequest, TimeSpan.FromSeconds(1));
                    break;
                default:
                    throw new NotSupportedException($"{method} is unsupported");
            }
        }


        [When("Generic request is responding")]
        public void GenericRequestIsResponding(Table dataTable)
        {
            _command = new Fixture().Create<Sample.Api.Sample.GenericCommand>();
            var problem = dataTable.AsProblem(_command.Value);

            _messageHandlerMock = _apiDriver.MockHttpSender("Generic", "http://generic-service/");
            _messageHandlerMock.SetupSend(_ => true, new HttpResponseMessage(problem.Status) { Content = JsonContent.Create(problem, new MediaTypeHeaderValue(MediaTypeNames.Application.Json.Problem)) });
        }

        [Then("Request should be sent to")]
        public void ShouldSentTo(Table dataTable)
        {
            var baseAddress = dataTable.GetValue<string>("BaseAddress");
            var method = dataTable.GetValue<string>("Method");
            var path = dataTable.GetValue<string>("Path");

            if (_command is not null) _messageHandlerMock.VerifySend(message => message.Verify(method!, baseAddress!, path!, _command));
            if (_query is not null) _messageHandlerMock.VerifySend(message => message.Verify<Sample.Api.Sample.GenericQuery, string>(method!, baseAddress!, path!, _query));
            if (_getRequest is not null) _messageHandlerMock.VerifySend(message => message.Verify(method!, baseAddress!, $"{path}?value={_getRequest.Value}"));
            if (_postRequest is not null) _messageHandlerMock.VerifySend(message => message.Verify(method!, baseAddress!, path!, new PostRequest.Parameter(_postRequest.Value)));
            if (_putRequest is not null) _messageHandlerMock.VerifySend(message => message.Verify(method!, baseAddress!, path!));
            if (_patchRequest is not null) _messageHandlerMock.VerifySend(message => message.Verify(method!, baseAddress!, path!));
            if (_deleteRequest is not null) _messageHandlerMock.VerifySend(message => message.Verify(method!, baseAddress!, path!));
        }


        [Then("Should be thrown {string}")]
        public async Task ShoudlBeThrown(string exception)
        {
            switch (exception)
            {
                case "NotFoundException":
                    await Assert.ThrowsAsync<NotFoundException>(() => _apiDriver.SendAsync(_command, TimeSpan.FromSeconds(1)));
                    break;
                case "HttpRequestException":
                    await Assert.ThrowsAsync<HttpRequestException>(() => _apiDriver.SendAsync(_command, TimeSpan.FromSeconds(1)));
                    break;
                case "ValidationException":
                    await Assert.ThrowsAsync<ValidationException>(() => _apiDriver.SendAsync(_command, TimeSpan.FromSeconds(1)));
                    break;
                case "VerificationException":
                    await Assert.ThrowsAsync<VerificationException>(() => _apiDriver.SendAsync(_command, TimeSpan.FromSeconds(1)));
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        #region Requests
        [Get("external-api/{Path}")]
        internal class GetRequest : IRequest
        {
            public string Path { get; } = "get-endpoint";

            [QueryParameter]
            public string Value { get; }

            public GetRequest(string value)
            {
                Value = value;
            }
        }

        [Post("external-api/{Path}")]
        internal class PostRequest : IRequest
        {
            public string Path { get; } = "post-endpoint";

            [BodyParameter]
            public string Value { get; }

            public PostRequest(string value)
            {
                Value = value;
            }

            public record Parameter
            {
                public string Value { get; }

                public Parameter(string value)
                {
                    Value = value;
                }
            }
        }

        [Put("external-api/{Path}")]
        internal class PutRequest : IRequest
        {
            public string Path { get; } = "put-endpoint";
        }

        [Patch("external-api/{Path}")]
        internal class PatchRequest : IRequest
        {
            public string Path { get; } = "patch-endpoint";
        }

        [Delete("external-api/{Path}")]
        internal class DeleteRequest : IRequest
        {
            public string Path { get; } = "delete-endpoint";
        }
        #endregion
    }

    internal static class RequestSendingExtensions
    {
        public static bool Verify(this HttpRequestMessage message, string method, string baseAddress, string path)
        {
            if (message.RequestUri is null) return false;

            return message.Method.Method.Equals(method)
                && message.RequestUri.AbsoluteUri.StartsWith($"{baseAddress}{path}");
        }

        public static bool Verify<TCommand>(this HttpRequestMessage message, string method, string baseAddress, string path, TCommand command)
            where TCommand : Command
        {
            var content = message.Content?.ReadFromJsonAsync<TCommand>().GetAwaiter().GetResult();
            if (content is null) return false;

            return message.Verify(method, baseAddress, path)
                && content.Equals(command);
        }

        public static bool Verify<TQuery, T>(this HttpRequestMessage message, string method, string baseAddress, string path, TQuery query)
            where TQuery : Query<T>
        {
            if (message.RequestUri is null) return false;

            return message.Verify(method, baseAddress, path)
                && QueryHelpers.ParseQuery(message.RequestUri.Query).SequenceEqual(query.AsQueryString());
        }

        public static bool Verify(this HttpRequestMessage message, string method, string baseAddress, string path, RequestSendingStepDefinitions.PostRequest.Parameter parameter)
        {
            var content = message.Content?.ReadFromJsonAsync<RequestSendingStepDefinitions.PostRequest.Parameter>().GetAwaiter().GetResult();
            if (content is null) return false;

            return message.Verify(method, baseAddress, path)
                && content.Equals(parameter);
        }
    }
}

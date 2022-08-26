using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using STrain.CQS.Test.Function.Drivers;
using STrain.CQS.Test.Function.Workarounds;
using STrain.Sample.Api;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Function.StepDefinitions
{
    [Binding]
    public class ErrorHandlingStepDefinitions
    {
        private readonly WebApplicationFactory<Program> _driver;
        private string _resource = null!;
        private HttpResponseMessage _response = null!;

        public ErrorHandlingStepDefinitions(ITestOutputHelper outputHelper)
        {
            _driver = new LightinjectWebApplicationFactory<Program>()
                        .Initialize(outputHelper);
        }

        [When("Throwing NotFoundException")]
        public async Task ThrowNotFoundExceptionAsync()
        {
            _resource = new Fixture().Create<string>();
            _response = await _driver.ReceiveCommandAsync(new SampleNotFoundCommand(_resource), TimeSpan.FromSeconds(1));
        }

        [When("Calling {string} endpoint")]
        public async Task SendingRequestToUnkownEndpointAsync(string endpoint)
        {
            _response = await _driver.SendAsync(endpoint, TimeSpan.FromSeconds(1));
        }

        [Then("Error response should be")]
        public async Task ErrorResponseShouldBe(Table dataTable)
        {
            var problem = dataTable.AsProblem(_resource);
            var code = dataTable.GetEnum<HttpStatusCode>("Status");
            var contentType = dataTable.GetValue<string>("ContentType");

            Assert.Equal(code, _response.StatusCode);
            Assert.Equal(contentType, _response.Content.Headers.ContentType?.MediaType);
            Assert.Equal(problem, await _response.Content.ReadFromJsonAsync<Problem>());
        }

        internal record Problem
        {
            public string Type { get; }
            public string Title { get; }
            public HttpStatusCode Status { get; }
            public string Detail { get; }
            public string Instance { get; }

            public Problem(string type, string title, HttpStatusCode status, string detail, string instance)
            {
                Type = type;
                Title = title;
                Status = status;
                Detail = detail;
                Instance = instance;
            }
        }
    }

    internal static class ErrorHandlingExtensions
    {
        public static ErrorHandlingStepDefinitions.Problem AsProblem(this Table dataTable, string resource)
        {
            return new ErrorHandlingStepDefinitions.Problem(dataTable.GetValue<string>("Type"),
                dataTable.GetValue<string>("Title"), dataTable.GetEnum<HttpStatusCode>("Status"),
                dataTable.GetValue<string>("Detail").Replace("{resource}", resource), dataTable.GetValue<string>("Instance"));
        }

        public static T GetValue<T>(this Table dataTable, string header)
            where T : class, IConvertible
        {
            return (T)Convert.ChangeType(dataTable.Rows[0][header], typeof(T));
        }

        public static TEnum GetEnum<TEnum>(this Table dataTable, string header)
            where TEnum : struct, Enum
        {
            return Enum.Parse<TEnum>(dataTable.Rows[0][header]);
        }
    }
}

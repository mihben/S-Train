using AutoFixture;
using Microsoft.AspNetCore.Mvc.Testing;
using STrain.CQS.Test.Function.Drivers;
using STrain.CQS.Test.Function.Support;
using STrain.Sample.Api;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using Xunit.Abstractions;
using Error = STrain.CQS.Test.Function.StepDefinitions.ErrorHandlingStepDefinitions.Problem.Error;

namespace STrain.CQS.Test.Function.StepDefinitions
{
    [Binding]
    public class ErrorHandlingStepDefinitions
    {
        private readonly WebApplicationFactory<Program> _driver;
        private readonly RequestContext _requestContext;

        public ErrorHandlingStepDefinitions(RequestContext requestContext, ITestOutputHelper outputHelper)
        {
            _driver = new LightinjectWebApplicationFactory<Program>()
                        .Initialize(outputHelper)
                        .MockAuthentication(builder => builder.Forbidden().Unathorized());
            _requestContext = requestContext;
        }

        [When("Throwing NotFoundException")]
        public async Task ThrowNotFoundExceptionAsync()
        {
            _requestContext.Request = new Fixture().Create<string>();
            _requestContext.Response = await _driver.ReceiveCommandAsync(new SampleNotFoundCommand(_requestContext.Request.ToString()!), TimeSpan.FromSeconds(1));
        }

        [When("Calling {string} endpoint")]
        public async Task SendingRequestToUnkownEndpointAsync(string endpoint)
        {
            _requestContext.Response = await _driver.SendAsync(endpoint, TimeSpan.FromSeconds(1));
        }

        internal class ProblemEqualityComparer : IEqualityComparer<Problem?>
        {
            public bool Equals(Problem? x, Problem? y)
            {
                return (x is null && y is null) ||
                    (x is not null
                    && y is not null
                    && x.Type.Equals(y.Type)
                    && x.Title.Equals(y.Title)
                    && x.Status.Equals(y.Status)
                    && x.Detail.Equals(y.Detail)
                    && x.Instance.Equals(y.Instance)
                    && ((x.Errors is null && y.Errors is null)
                        || (x.Errors?.SequenceEqual(y.Errors ?? Enumerable.Empty<Error>()) ?? false))
                    );
            }

            public int GetHashCode([DisallowNull] Problem obj)
            {
                return HashCode.Combine(obj);
            }
        }

        internal record Problem
        {
            public string Type { get; }
            public string Title { get; }
            public HttpStatusCode Status { get; }
            public string Detail { get; }
            public string Instance { get; }
            public IEnumerable<Error>? Errors { get; }

            public Problem(string type, string title, HttpStatusCode status, string detail, string instance, IEnumerable<Error>? errors)
            {
                Type = type;
                Title = title;
                Status = status;
                Detail = detail;
                Instance = instance;
                Errors = errors;
            }

            public record Error
            {
                public string Property { get; }
                public string Message { get; }

                public Error(string property, string message)
                {
                    Property = property;
                    Message = message;
                }
            }
        }
    }

    internal static class ErrorHandlingExtensions
    {
        public static ErrorHandlingStepDefinitions.Problem AsProblem(this Table dataTable, string resource)
        {
            IEnumerable<Error>? errors = null;
            if (dataTable.Rows[0].TryGetValue("Errors.Property", out var property)) errors = new List<Error> { new Error(property, dataTable.GetValue<string>("Errors.Message")) };

            return new ErrorHandlingStepDefinitions.Problem(dataTable.GetValue<string>("Type"),
                dataTable.GetValue<string>("Title"), dataTable.GetEnum<HttpStatusCode>("Status"),
                dataTable.GetValue<string>("Detail").Replace("{resource}", resource), dataTable.GetValue<string>("Instance"), errors);
        }

        public static T? GetValue<T>(this Table dataTable, string header)
            where T : class, IConvertible
        {
            if (!dataTable.Rows[0].ContainsKey(header)) return null;
            return (T)Convert.ChangeType(dataTable.Rows[0][header], typeof(T));
        }

        public static TEnum GetEnum<TEnum>(this Table dataTable, string header)
            where TEnum : struct, Enum
        {
            return Enum.Parse<TEnum>(dataTable.Rows[0][header]);
        }
    }
}

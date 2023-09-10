using STrain.CQS.Test.Function.Drivers;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Json;

namespace STrain.CQS.Test.Function.StepDefinitions
{
    [Binding]
    public class ErrorHandlingStepDefinitions
    {
        private readonly ApiDriver _driver;

        private string _resource = null!;
        private HttpResponseMessage _response = null!;

        public ErrorHandlingStepDefinitions(ApiDriver driver)
        {
            _driver = driver;
        }

        [When("Calling {string} endpoint")]
        public async Task CallingAsync(string endpoint)
        {
            _resource = "NotFoundResource";
            _response = await _driver.GetAsync(endpoint, TimeSpan.FromSeconds(2));
        }

        [Then("Error response should be")]
        public async Task ShouldBeErrorResponseAsync(Table dataTable)
        {
            Assert.Equal(Enum.Parse<HttpStatusCode>(dataTable.GetValue<string>("Code")!)!, _response.StatusCode);
            Assert.Equal(dataTable.AsProblem(_resource), await _response.Content.ReadFromJsonAsync<Problem>(), new ProblemEqualityComparer());
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
                        || (x.Errors?.SequenceEqual(y.Errors ?? Enumerable.Empty<Problem.Error>()) ?? false))
                    );
            }

            public int GetHashCode([DisallowNull] Problem obj)
            {
                return HashCode.Combine(obj);
            }
        }
    }
}

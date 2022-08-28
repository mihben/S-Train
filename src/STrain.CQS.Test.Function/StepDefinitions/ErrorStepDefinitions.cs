using STrain.CQS.Test.Function.Support;
using System.Net;
using System.Net.Http.Json;
using static STrain.CQS.Test.Function.StepDefinitions.ErrorHandlingStepDefinitions;

namespace STrain.CQS.Test.Function.StepDefinitions
{
    [Binding]
    public class ErrorStepDefinitions
    {
        private readonly RequestContext _requestContext;

        public ErrorStepDefinitions(RequestContext requestContext)
        {
            _requestContext = requestContext;
        }


        [Then("Error response should be")]
        public async Task ErrorResponseShouldBe(Table dataTable)
        {
            var problem = dataTable.AsProblem(_requestContext.Parameter.ToString());
            var code = dataTable.GetEnum<HttpStatusCode>("Status");
            var contentType = dataTable.GetValue<string>("ContentType");

            Assert.Equal(code, _requestContext.Response.StatusCode);
            Assert.Equal(contentType, _requestContext.Response.Content.Headers.ContentType?.MediaType);
            Assert.Equal(problem, await _requestContext.Response.Content.ReadFromJsonAsync<Problem>(), new ProblemEqualityComparer());
        }
    }
}

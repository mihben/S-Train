using STrain.CQS;

namespace STrain.Sample.Api
{
    [Get()]
    public class ExternalSampleRequest : IRequest
    {
        [QueryParameter(Name = "q")]
        public string Criteria { get; }

        public ExternalSampleRequest(string criteria)
        {
            Criteria = criteria;
        }
    }
}

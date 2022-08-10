using STrain.Sample.Api;

namespace STrain.Sample.Backend.Performers
{
    public class SampleQueryPerformer : IQueryPerformer<SampleQuery, string>
    {
        public Task<string> PerformAsync(SampleQuery query, CancellationToken cancellationToken)
        {
            return Task.FromResult("SampleQuery called");
        }
    }
}

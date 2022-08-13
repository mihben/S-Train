using STrain.Sample.Api;

namespace STrain.Sample.Backend.Performers
{
    public class SampleQueryPerformer : IQueryPerformer<SampleQuery, SampleQuery.Result>
    {
        public Task<SampleQuery.Result> PerformAsync(SampleQuery query, CancellationToken cancellationToken)
        {
            return Task.FromResult(new SampleQuery.Result("Handled"));
        }
    }
}

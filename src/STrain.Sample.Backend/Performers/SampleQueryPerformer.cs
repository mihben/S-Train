namespace STrain.Sample.Backend.Performers
{
    public class SampleQueryPerformer : IQueryPerformer<Api.Sample.GenericQuery, string>
    {
        public Task<string> PerformAsync(Api.Sample.GenericQuery query, CancellationToken cancellationToken)
        {
            return Task.FromResult("Handled by performer");
        }
    }
}

namespace STrain.Sample.Api
{
    public record SampleQuery : Query<SampleQuery.Result>
    {
        public string Value { get; }

        public SampleQuery(string value)
        {
            Value = value;
        }

        public class Result
        {
            public string Response { get; }

            public Result(string response)
            {
                Response = response;
            }
        }
    }
}

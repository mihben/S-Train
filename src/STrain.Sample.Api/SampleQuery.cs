namespace STrain.Sample.Api
{
    [Get("4f4acd30-0ca3-4d69-8c98-4de87ad1223b")]
    public record SampleQuery : Query<SampleQuery.Result>
    {
        [BodyParameter]
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

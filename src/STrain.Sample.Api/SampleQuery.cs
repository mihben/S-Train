namespace STrain.Sample.Api
{
    public record SampleQuery : Query<string>
    {
        public string Value { get; }

        public SampleQuery(string value)
        {
            Value = value;
        }
    }
}

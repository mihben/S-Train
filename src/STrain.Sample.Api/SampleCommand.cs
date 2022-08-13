namespace STrain.Sample.Api
{
    [Get("search")]
    public record SampleCommand : Command
    {
        [QueryParameter(Name = "q")]
        public string Parameter { get; }

        public SampleCommand(string parameter)
        {
            Parameter = parameter;
        }
    }
}
namespace STrain.Sample.Api
{
    public record SampleCommand : Command
    {
        public string Parameter { get; }

        public SampleCommand(string parameter)
        {
            Parameter = parameter;
        }
    }
}
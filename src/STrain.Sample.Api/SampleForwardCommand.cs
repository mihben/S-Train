namespace STrain.Sample.Api
{
    public record SampleForwardCommand : Command
    {
        public string Resource { get; }

        public SampleForwardCommand(string resource)
        {
            Resource = resource;
        }
    }
}

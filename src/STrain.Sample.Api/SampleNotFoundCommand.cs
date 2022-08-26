namespace STrain.Sample.Api
{
    public record SampleNotFoundCommand : Command
    {
        public string Resource { get; }

        public SampleNotFoundCommand(string resource)
        {
            Resource = resource;
        }
    }
}

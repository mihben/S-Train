namespace STrain.Sample.Api
{
    public static class Error
    {
        public record NotFoundCommand : Command
        {
            public string Resource { get; }

            public NotFoundCommand(string resource)
            {
                Resource = resource;
            }
        }
    }
}

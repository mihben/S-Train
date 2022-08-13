namespace STrain.CQS.NetCore.RequestSending
{
    public record HttpRequestSenderOptions
    {
        public Uri BaseAddress { get; init; } = null!;
        public string Path { get; init; } = null!;
    }
}

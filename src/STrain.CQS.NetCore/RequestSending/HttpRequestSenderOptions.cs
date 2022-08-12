namespace STrain.CQS.NetCore.RequestSending
{
    public record HttpRequestSenderOptions
    {
        public string Path { get; } = null!;
    }
}

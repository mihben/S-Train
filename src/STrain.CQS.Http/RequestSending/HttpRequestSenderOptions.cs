namespace STrain.CQS.Http.RequestSending
{
    public record HttpRequestSenderOptions
    {
        public Uri BaseAddress { get; set; } = null!;
        public string? Path { get; set; }
    }
}

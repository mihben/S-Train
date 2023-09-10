namespace STrain.CQS.Test.Unit.Supports
{
    internal record PropertyParameterRequest : IRequest
    {
        [BodyParameter]
        public string ByName { get; set; } = null!;
        [BodyParameter(Name = "by-attribute")]
        public string ByAttribute { get; set; } = null!;
        public string NotToBeSerialized { get; set; } = null!;
    }
}

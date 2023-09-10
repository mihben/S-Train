namespace STrain.CQS.Test.Unit.Supports
{
    [BodyParameter]
    internal record BodyParameterRequest : IRequest
    {
        public string ByName { get; set; } = null!;
        [BodyParameter(Name = "by-attribute")]
        public string ByAttribute { get; set; } = null!;
    }
}
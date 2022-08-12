namespace STrain.CQS.Test.Unit.Supports
{
    [Patch("test-path/{parameter}")]
    public record TestExternalWithAttributeCommand : Command
    {
        public string Parameter { get; }

        public TestExternalWithAttributeCommand(string parameter)
        {
            Parameter = parameter;
        }
    }
}

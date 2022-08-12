namespace STrain.CQS.Test.Unit.Supports
{
    [Path("test-path/{parameter}")]
    public record TestExternalWithAttributeCommand : Command
    {
        public string Parameter { get; }

        public TestExternalWithAttributeCommand(string parameter)
        {
            Parameter = parameter;
        }
    }
}

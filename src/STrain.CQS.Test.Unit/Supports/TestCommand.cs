namespace STrain.CQS.Test.Unit.Supports
{
    public record TestCommand : Command
    {
        public string Parameter { get; }

        public TestCommand(string parameter)
        {
            Parameter = parameter;
        }
    }
}

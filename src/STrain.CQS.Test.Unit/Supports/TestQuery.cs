namespace STrain.CQS.Test.Unit.Supports
{
    public record TestQuery : Query<string>
    {
        public string Parameter { get; }

        public TestQuery(string parameter)
        {
            Parameter = parameter;
        }
    }
}

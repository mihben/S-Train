namespace STrain.CQS.Test.Unit.Supports
{
    public record TestQuery : Query<string>
    {
        public Guid Guid { get; }
        public int Int { get; }
        public double Double { get; }
        public long Long { get; }
        public string String { get; }
        public bool Bool { get; }
        public char Char { get; }
        public IEnumerable<int> Collection { get; }

        public TestQuery(Guid guid, int @int, double @double, long @long, string @string, bool @bool, char @char, IEnumerable<int> collection)
        {
            Guid = guid;
            Int = @int;
            Double = @double;
            Long = @long;
            String = @string;
            Bool = @bool;
            Char = @char;
            Collection = collection;
        }
    }
}

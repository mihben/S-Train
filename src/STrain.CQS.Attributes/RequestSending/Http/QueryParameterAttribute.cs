namespace STrain.CQS.Attributes.RequestSending.Http
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class QueryParameterAttribute : Attribute
    {
        public string? Name { get; set; }
    }
}

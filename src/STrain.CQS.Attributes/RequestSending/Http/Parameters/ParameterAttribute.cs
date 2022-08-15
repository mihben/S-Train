namespace STrain.CQS.Attributes.RequestSending.Http.Parameters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ParameterAttribute : Attribute
    {
        public string? Name { get; set; }
    }
}

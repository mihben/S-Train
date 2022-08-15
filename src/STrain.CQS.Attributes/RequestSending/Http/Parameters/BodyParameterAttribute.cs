using STrain.CQS.Attributes.RequestSending.Http.Parameters;

namespace STrain
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class BodyParameterAttribute : ParameterAttribute
    {
    }
}

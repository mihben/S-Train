using STrain.CQS.Attributes.RequestSending.Http;

namespace STrain
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class GetAttribute : MethodAttribute
    {
        public GetAttribute()
            : this(string.Empty)
        {

        }

        public GetAttribute(string path)
            : base(path, HttpMethod.Get)
        {
        }
    }
}

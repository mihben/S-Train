using STrain.CQS.Attributes.RequestSending.Http;

namespace STrain
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PostAttribute : MethodAttribute
    {
        public PostAttribute(string path)
            : base(path, HttpMethod.Post)
        {
        }
    }
}

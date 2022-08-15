namespace STrain.CQS.Attributes.RequestSending.Http
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class MethodAttribute : RouteAttribute
    {
        public HttpMethod Method { get; set; }

        public MethodAttribute(string path, HttpMethod method)
            : base(path)
        {
            Method = method;
        }
    }
}

namespace STrain
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RouteAttribute : Attribute
    {
        public string Path { get; }

        public RouteAttribute(string path)
        {
            Path = path;
        }
    }
}

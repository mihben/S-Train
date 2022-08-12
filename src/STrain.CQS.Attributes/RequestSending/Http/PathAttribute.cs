namespace STrain
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PathAttribute : Attribute
    {
        public string Path { get; }

        public PathAttribute(string path)
        {
            Path = path;
        }
    }
}

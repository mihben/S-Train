namespace STrain.CQS.NetCore.RequestSending
{
    public class PathContainer : IPathProvider, IPathRegister
    {
        private readonly IDictionary<Type, string> _registrations = new Dictionary<Type, string>();

        public string this[Type type]
        {
            get
            {
                _registrations.TryGetValue(type, out var path);
                if (path is null)
                {
                    var result = _registrations.FirstOrDefault(r => r.Key.IsAssignableFrom(type));
                    if (result.Equals(default(KeyValuePair<Type, string>))) throw new InvalidOperationException($"Path is not found for {type}");
                    path = result.Value;
                }

                return path;
            }
        }

        public void Registrate<TRequest>(string path)
            where TRequest : IRequest
        {
            if (path is null)  throw new ArgumentNullException(nameof(path));
            if (_registrations.ContainsKey(typeof(TRequest))) throw new ArgumentException($"Path for {typeof(TRequest)} has already been registrated");
            if (path.StartsWith('/')) throw new ArgumentException("Path must not start with '/'");

            _registrations.Add(typeof(TRequest), path);
        }
    }
}

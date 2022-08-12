namespace STrain.CQS.NetCore.RequestSending
{
    public class MethodContainer : IMethodProvider, IMethodRegister
    {
        private readonly IDictionary<Type, HttpMethod> _registrations = new Dictionary<Type, HttpMethod>();

        public HttpMethod this[Type type]
        {
            get
            {
                _registrations.TryGetValue(type, out HttpMethod? method);
                if (method is null)
                {
                    var result = _registrations.FirstOrDefault(r => r.Key.IsAssignableFrom(type));
                    if (result.Equals(default(KeyValuePair<Type, HttpMethod>))) throw new InvalidOperationException($"Method is not found for {type}");
                    method = result.Value;
                }

                return method;
            }
        }

        public void Registrate<TRequest>(HttpMethod method)
            where TRequest : IRequest
        {
            if (method is null) throw new ArgumentNullException(nameof(method));
            if (_registrations.ContainsKey(typeof(TRequest))) throw new ArgumentException($"Method for {typeof(TRequest)} has already beed registrated");

            _registrations.Add(typeof(TRequest), method);
        }
    }
}

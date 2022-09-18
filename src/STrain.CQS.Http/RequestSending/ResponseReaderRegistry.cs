using STrain.CQS.Http.RequestSending.Readers;

namespace STrain.CQS.Http.RequestSending
{
    public class ResponseReaderRegistry : IResponseReaderProvider, IResponseReaderRegister
    {
        private readonly IDictionary<string, Type> _registrations = new Dictionary<string, Type>();

        public Type this[string? mediaType]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(mediaType)) throw new ArgumentException($"'{nameof(mediaType)}' cannot be null or whitespace.");

                if (!_registrations.TryGetValue(mediaType, out var result)) throw new InvalidOperationException($"Reader is not registered for {mediaType} media type.");
                return result;
            }
        }

        public IResponseReaderRegister Registrate<TResponseReader>(string mediaType)
            where TResponseReader : IResponseReader => Registrate(mediaType, typeof(TResponseReader));
        public IResponseReaderRegister Registrate(string mediaType, Type type)
        {
            if (string.IsNullOrWhiteSpace(mediaType)) throw new ArgumentException($"'{nameof(mediaType)}' cannot be null or whitespace.", nameof(mediaType));
            if (type is null
                || !type.IsAssignableTo(typeof(IResponseReader))) throw new InvalidOperationException("Type cannot be null and must implement 'IResponseReader' interface.");

            if (_registrations.ContainsKey(mediaType)) _registrations[mediaType] = type;
            else _registrations.Add(mediaType, type);
            return this;
        }
    }
}

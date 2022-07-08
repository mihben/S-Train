using System.Net.Http.Headers;

namespace STrain
{
    public class Command : IRequest, IEquatable<Command>
    {
        public Guid RequestId { get; }

        public Command(Guid? requestId = null)
        {
            RequestId = requestId ?? Guid.NewGuid();
        }

        public bool Equals(Command? other) => RequestId.Equals(other?.RequestId);
        public override bool Equals(object? obj) => Equals(obj as Command);
        public override int GetHashCode() => RequestId.GetHashCode();
    }
}

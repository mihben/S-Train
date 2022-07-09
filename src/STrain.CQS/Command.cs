using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;

namespace STrain
{
    public class Command : IRequest, IEqualityComparer<Command>
    {
        public Guid RequestId { get; }

        public Command(Guid? requestId = null)
        {
            RequestId = requestId ?? Guid.NewGuid();
        }

        public override bool Equals(object? obj) => Equals(this, obj as Command);
        public override int GetHashCode() => GetHashCode(this);

        public bool Equals(Command? x, Command? y)
        {
            return (x is null) && (y is null)
                || (x?.RequestId.Equals(y?.RequestId) ?? false);
        }
        public int GetHashCode([DisallowNull] Command obj) => obj.RequestId.GetHashCode();
    }
}

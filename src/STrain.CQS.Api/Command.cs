using System.Diagnostics.CodeAnalysis;

namespace STrain
{
    /// <summary>
    /// Represents a command.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Command : IRequest, IEqualityComparer<Command>
    {
        /// <summary>
        /// Unique identifier of the <see cref="Command"/>.
        /// </summary>
        public Guid RequestId { get; }

        /// <summary>
        /// Create new <see cref="Command"/> instance.
        /// </summary>
        /// <param name="requestId">Unique idenftifier of the <see cref="Command"/>. Will be generated if it is <see cref="null"/></param>
        public Command(Guid? requestId = null)
        {
            RequestId = requestId ?? Guid.NewGuid();
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => Equals(this, obj as Command);
        /// <inheritdoc/>
        public override int GetHashCode() => GetHashCode(this);

        /// <inheritdoc/>
        public bool Equals(Command? x, Command? y)
        {
            return (x is null) && (y is null)
                || (x?.RequestId.Equals(y?.RequestId) ?? false);
        }
        /// <inheritdoc/>
        public int GetHashCode([DisallowNull] Command obj) => obj.RequestId.GetHashCode();
    }
}

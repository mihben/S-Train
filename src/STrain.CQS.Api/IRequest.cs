namespace STrain.CQS
{
    /// <summary>
    /// Represent a request.
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// Unique identifier of the request.
        /// </summary>
        Guid RequestId { get; }
    }
}

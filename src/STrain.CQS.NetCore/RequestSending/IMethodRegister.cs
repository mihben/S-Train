namespace STrain.CQS.NetCore.RequestSending
{
    public interface IMethodRegister
    {
        void Registrate<TRequest>(HttpMethod method)
            where TRequest : IRequest;
    }
}

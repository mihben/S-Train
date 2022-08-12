namespace STrain.CQS.NetCore.RequestSending
{
    public interface IPathRegister
    {
        void Registrate<TRequest>(string path)
            where TRequest : IRequest;
    }
}

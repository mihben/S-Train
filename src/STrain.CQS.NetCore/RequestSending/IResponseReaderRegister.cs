using STrain.CQS.NetCore.RequestSending.Parsers;

namespace STrain.CQS.NetCore.RequestSending
{
    public interface IResponseReaderRegister
    {
        IResponseReaderRegister Registrate<TResponseReader>(string meditType) where TResponseReader : IResponseReader;
    }
}
